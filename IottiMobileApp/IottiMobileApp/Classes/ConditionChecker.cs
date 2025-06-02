using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace IottiMobileApp.Classes
{
    // Enum per i tipi di condizioni
    public enum ConditionType
    {
        NetworkConnection,
        DiskSpace,
        DatabaseConnection,
        ServiceAvailability,
        Custom
    }

    // Enum per i livelli di severità
    public enum SeverityLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }

    // Classe per rappresentare il risultato di un check
    public class ConditionResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public ConditionType Type { get; set; }
        public SeverityLevel Severity { get; set; }
        public DateTime CheckTime { get; set; }
        public Exception? Exception { get; set; }

        public ConditionResult()
        {
            CheckTime = DateTime.Now;
        }
    }

    public class ConditionChecker
    {
        private readonly IToastService _toastService;
        private readonly List<Func<Task<ConditionResult>>> _checks;

        public ConditionChecker(IToastService toastService)
        {
            _toastService = toastService ?? throw new ArgumentNullException(nameof(toastService));
            _checks = new List<Func<Task<ConditionResult>>>();
        }

        // Metodo per aggiungere check personalizzati
        public void AddCheck(Func<Task<ConditionResult>> checkFunction)
        {
            _checks.Add(checkFunction);
        }

        // Check della connessione di rete
        public async Task<ConditionResult> CheckNetworkConnectionAsync()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync("8.8.8.8", 3000);

                    if (reply.Status == IPStatus.Success)
                    {
                        return new ConditionResult
                        {
                            IsSuccess = true,
                            Message = "Connessione di rete disponibile",
                            Type = ConditionType.NetworkConnection,
                            Severity = SeverityLevel.Info
                        };
                    }
                    else
                    {
                        return new ConditionResult
                        {
                            IsSuccess = false,
                            Message = "Connessione di rete non disponibile",
                            Type = ConditionType.NetworkConnection,
                            Severity = SeverityLevel.Error
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ConditionResult
                {
                    IsSuccess = false,
                    Message = "Errore durante il controllo della connessione di rete",
                    Type = ConditionType.NetworkConnection,
                    Severity = SeverityLevel.Error,
                    Exception = ex
                };
            }
        }

        // Check dello spazio su disco
        public async Task<ConditionResult> CheckDiskSpaceAsync(string driveLetter = "C", long minSpaceGB = 1)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var drive = new DriveInfo(driveLetter);
                    var freeSpaceGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024);

                    if (freeSpaceGB >= minSpaceGB)
                    {
                        return new ConditionResult
                        {
                            IsSuccess = true,
                            Message = $"Spazio su disco sufficiente: {freeSpaceGB}GB liberi",
                            Type = ConditionType.DiskSpace,
                            Severity = SeverityLevel.Info
                        };
                    }
                    else
                    {
                        return new ConditionResult
                        {
                            IsSuccess = false,
                            Message = $"Spazio su disco insufficiente: solo {freeSpaceGB}GB liberi",
                            Type = ConditionType.DiskSpace,
                            Severity = SeverityLevel.Warning
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new ConditionResult
                    {
                        IsSuccess = false,
                        Message = "Errore durante il controllo dello spazio su disco",
                        Type = ConditionType.DiskSpace,
                        Severity = SeverityLevel.Error,
                        Exception = ex
                    };
                }
            });
        }

        // Metodo per eseguire tutti i check e notificare automaticamente
        public async Task RunAllChecksAsync(bool notifyOnSuccess = false)
        {
            var results = new List<ConditionResult>();

            // Esegui i check predefiniti
            results.Add(await CheckNetworkConnectionAsync());
            results.Add(await CheckDiskSpaceAsync());

            // Esegui i check personalizzati
            foreach (var check in _checks)
            {
                try
                {
                    results.Add(await check());
                }
                catch (Exception ex)
                {
                    results.Add(new ConditionResult
                    {
                        IsSuccess = false,
                        Message = "Errore durante l'esecuzione di un check personalizzato",
                        Type = ConditionType.Custom,
                        Severity = SeverityLevel.Error,
                        Exception = ex
                    });
                }
            }

            // Notifica i risultati
            await NotifyResultsAsync(results, notifyOnSuccess);
        }

        // Metodo per eseguire un singolo check e notificare
        public async Task<ConditionResult> RunSingleCheckAsync<T>(Func<Task<ConditionResult>> checkFunction, bool notify = true)
        {
            var result = await checkFunction();

            if (notify)
            {
                await NotifyResultAsync(result);
            }

            return result;
        }

        // Metodo privato per notificare i risultati
        private async Task NotifyResultsAsync(List<ConditionResult> results, bool notifyOnSuccess)
        {
            foreach (var result in results)
            {
                if (!result.IsSuccess || notifyOnSuccess)
                {
                    await NotifyResultAsync(result);
                }
            }
        }

        // Metodo privato per notificare un singolo risultato
        private async Task NotifyResultAsync(ConditionResult result)
        {
            switch (result.Severity)
            {
                case SeverityLevel.Info:
                    if (result.IsSuccess)
                        await _toastService.ShowSuccessAsync(result.Message);
                    else
                        await _toastService.ShowInfoAsync(result.Message);
                    break;

                case SeverityLevel.Warning:
                    await _toastService.ShowWarningAsync(result.Message);
                    break;

                case SeverityLevel.Error:
                case SeverityLevel.Critical:
                    await _toastService.ShowErrorAsync(result.Message);
                    break;
            }
        }
    }
}
