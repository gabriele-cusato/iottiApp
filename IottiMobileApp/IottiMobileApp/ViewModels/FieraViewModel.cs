using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbMobileModel.Models.IntermediateDb;
using DbMobileModel.Services.Interfaces;
using IottiMobileApp.Classes;

namespace IottiMobileApp.ViewModels
{
    public partial class FieraViewModel : ObservableObject
    {
        private readonly IIntermediateDbService _intermediateDbService;

        public FieraViewModel(IIntermediateDbService intermediateDbService)
        {
            _intermediateDbService = intermediateDbService;

            // Carica dalla sessione prima di caricare i dati
            LoadFromSession();

            // Avvia il caricamento in modo sicuro
            _ = LoadDataAsync();
        }

        [ObservableProperty]
        public ObservableCollection<MissioneTes> fiereOP = new();

        [ObservableProperty]
        private MissioneTes? fieraSelezionata;

        [ObservableProperty]
        bool isRefreshing;

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            List<MissioneTes>? listaFiere = new List<MissioneTes>();
            try
            {
                listaFiere = await _intermediateDbService.GetAllFiereAsync();
                if (listaFiere != null && listaFiere.Any())
                {
                    foreach (var f in listaFiere)
                        Debug.WriteLine($"Fiera: {f.MteSapProjectName}");
                    FiereOP.Clear();
                    foreach (var fiera in listaFiere)
                        FiereOP.Add(fiera);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("errore: " + ex);
            }
            finally
            {
                if (IsRefreshing)
                    IsRefreshing = false;
            }
        }

        /// <summary>
        /// Metodo standardizzato chiamato da SmartExpander quando viene selezionato un elemento
        /// Questo metodo DEVE esistere in tutti i ViewModel che usano SmartExpander
        /// </summary>
        /// <param name="selectedItem">L'oggetto selezionato dall'expander</param>
        private void OnSmartExpanderSelectionChanged(object selectedItem)
        {
            // Gestisce la sessione in base al tipo di oggetto selezionato
            if (selectedItem is MissioneTes fiera)
            {
                // Salva la fiera in sessione
                UserSession.FieraSelezionata = fiera;
                System.Diagnostics.Debug.WriteLine($"FieraViewModel: Salvata fiera in sessione = {fiera.MteSapProjectName}");
            }
        }

        /// <summary>
        /// Carica i valori dalla sessione e li imposta nel ViewModel
        /// Chiamato nel costruttore e quando serve ricaricare
        /// </summary>
        public void LoadFromSession()
        {
            // Carica fiera dalla sessione
            if (UserSession.FieraSelezionata != null)
            {
                FieraSelezionata = UserSession.FieraSelezionata;
                System.Diagnostics.Debug.WriteLine($"FieraViewModel: Caricata fiera dalla sessione = {UserSession.FieraSelezionata.MteSapProjectName}");
            }
        }

        /// <summary>
        /// Comando per ricaricare dalla sessione (se serve chiamarlo da UI)
        /// </summary>
        [RelayCommand]
        public void ReloadFromSession()
        {
            LoadFromSession();
        }

        //[RelayCommand]
        //private async Task LoadDataAsync()
        //{
        //    try
        //    {
        //        // TEMPORANEO: Genera dati di test invece di caricare dal database
        //        var listaFiere = GenerateTestData();

        //        if (listaFiere != null && listaFiere.Any())
        //        {
        //            foreach (var f in listaFiere)
        //                Debug.WriteLine($"Fiera: {f.MteSapProjectName}");

        //            FiereOP.Clear();
        //            foreach (var fiera in listaFiere)
        //                FiereOP.Add(fiera);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("errore: " + ex);
        //    }
        //    finally
        //    {
        //        if (IsRefreshing)
        //            IsRefreshing = false;
        //    }
        //}

        //// METODO TEMPORANEO: Genera dati di test
        //private List<MissioneTes> GenerateTestData()
        //{
        //    var testData = new List<MissioneTes>();
        //    var random = new Random();

        //    // Nomi di fiere realistiche per il test
        //    var nomiFiere = new[]
        //    {
        //        "Fiera del Mobile Milano",
        //        "Salone dell'Auto Torino",
        //        "Expo Tecnologia Roma",
        //        "Fiera Agricoltura Verona",
        //        "Mostra Artigianato Firenze",
        //        "Salone Nautico Genova",
        //        "Fiera Enogastronomia Bologna",
        //        "Expo Moda Milano",
        //        "Salone Bellezza Rimini",
        //        "Fiera Edilizia Padova",
        //        "Mostra Antiquariato Lucca",
        //        "Salone Energia Rinnovabile",
        //        "Fiera Turismo Venezia",
        //        "Expo Elettronica Torino",
        //        "Salone Arte Contemporanea",
        //        "Fiera Medicale Milano",
        //        "Mostra Libri Bologna",
        //        "Salone Gioielli Vicenza",
        //        "Fiera Robotica Genova",
        //        "Expo Sostenibilità Roma",
        //        "Salone Design Furniture",
        //        "Fiera Cosmetica Milano",
        //        "Mostra Fotografia Torino",
        //        "Salone Vini Verona",
        //        "Fiera Tessile Como",
        //        "Expo Gaming Milano",
        //        "Salone Cucina Modena",
        //        "Fiera del Mobile Milano",
        //        "Fiera del Mobile Milano",
        //        "Fiera del Mobile Milano",
        //        "Fiera del Mobile Milano",
        //        "Fiera del Mobile Milano",
        //        "Fiera del Mobile Milano",
        //        "Fiera del Mobile Milano",
        //        "Fiera del Mobile Milano",
        //        "Fiera del Mobile Milano",
        //        "Fiera del Mobile Milano",
        //        "Fiera Startup Innovation"
        //    };

        //    for (int i = 0; i < nomiFiere.Length; i++)
        //    {
        //        testData.Add(new MissioneTes
        //        {
        //            MteMissioneTesId = i + 1,
        //            MteSerieNumero = $"F{2024}",
        //            MteAnno = "2024",
        //            MteNumero = i + 1,
        //            MteDataMissione = DateTime.Now.AddDays(random.Next(-30, 90)), // Date random tra 30 giorni fa e 90 giorni nel futuro
        //            MteDescrizione = $"Partecipazione alla {nomiFiere[i]}",
        //            MteSapProjectCode = $"PRJ{(i + 1):D3}",
        //            MteSapProjectName = nomiFiere[i],
        //            MteCreateDt = DateTime.Now.AddDays(-random.Next(1, 365)),
        //            MteUpdateDt = DateTime.Now.AddDays(-random.Next(0, 30))
        //        });
        //    }

        //    Debug.WriteLine($"Generati {testData.Count} elementi di test per le fiere");
        //    return testData;
        //}
    }
}