using System.Reflection;
using System.Text;
using DbMobileModel;
using Microsoft.Extensions.Configuration;

namespace IottiMobileApp.Classes
{
    internal class ResourcesHelper
    {
        private static string settingsFileName  = MauiProgram.AppConfig.SettingsFileName; //variabile di configurazione per il nome del file di config
        private static string embeddedDbFolder  = MauiProgram.AppConfig.EmbeddedDbFolder; //variabile di configurazione per il percorso del db da caricare
        private static string mauiAppName       = String.Empty;                 //variabile per memorizzare il nome dell'app maui da utilizzare nel modulo db
        private static string appData           = FileSystem.AppDataDirectory;  //variabile per calcolare una sola vola il percorso della cartella app data dell'applicazione

        internal static async void StartInitialization()
        {
            try
            {
                await AppDataConfigAsync();
            }
            catch (Exception ex)
            { 
                Console.WriteLine($"Errore nella copia dei file nella cartella AppData: {ex}");
            }

            try
            {
                await InitializeConfigAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'inizializzazione del db: {ex}");
            }

            //await DbConnection.StartDbConnection(mauiAppName, configStream, GetDbStream);
        }

        /// <summary>
        /// mette il file di configurazione in una cartella scrivibile nel dispostivo mobile al primo avvio per poterlo poi modificare in produzione
        /// </summary>
        /// <returns></returns>
        private static async Task AppDataConfigAsync()
        {
            string destPath = Path.Combine(appData, settingsFileName);

            //se il file non esiste in memoria o ne è stato caricato uno aggiornato dalle risorse embedded, allora viene caricato
            if (!File.Exists(destPath) || MauiProgram.AppConfig.fileFromResource)
            {
                // 1) Ottieni l'assembly MAUI corrente
                var asm = typeof(App).Assembly;

                // 2) Calcola il nome completo della risorsa incorporata
                //    Assumo il default namespace "IottiMobileApp"
                string resourceName = $"{asm.GetName().Name}.{settingsFileName}";

                // 3) Apri lo stream dalla risorsa incorporata
                using Stream? src = asm.GetManifestResourceStream(resourceName)
                    ?? throw new FileNotFoundException(
                        $"Risorsa embedded '{resourceName}' non trovata in {asm.FullName}.");

                // 4) Copia il contenuto in AppDataDirectory
                using var dst = File.Create(destPath);
                await src.CopyToAsync(dst);
            }
        }

        /// <summary>
        /// Il database è una risorsa embedded, e quindi siccome la gestione del database è in un altro modulo, bisogna passare a startDbConnection i parametri di configurazione per ottenerlo.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        private static async Task InitializeConfigAsync()
        {
            //ottengo il nome dell'applicazione maui
            var mauiAsm = typeof(App).Assembly;
            mauiAppName = mauiAsm.GetName().Name!;

            string? dbFileName = MauiProgram.AppConfig.LocalServerConnection;    //nome del file del db sqlite (uguale tra risorsa embeddata e quello eventualmente gia caricato)
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, dbFileName!);     //path del db che dovrebbe essere già caricato, qui non so ancora se il file esiste gia

            string embeddedDbResourceName = $"{mauiAppName}.{embeddedDbFolder}.{dbFileName}";   //nome del db embeddato (riconosciuto da una risorsa che è una specie di percorso)
            using var dbStream = getEmbeddedDatabaseStream(embeddedDbResourceName, mauiAsm);    //recupero lo stream del db embeddato utilizzando la risorsa   

            //qui vuol dire che la risorsa non è stata caricata correttamente
            if (dbStream == null)
            {
                //non mettendo l'eccezione qui significherebbe che accetto che la risorsa non c'è, magari perchè non la voglio caricare
                throw new FileNotFoundException($"Database incorporato '{embeddedDbResourceName}' non trovato.");
            }

            //la chiamata piu giusta sarebbe stata senza passare il dbPath (calcolandolo dentro la funzione), però
            //dbPath viene passato perchè per calcolare l'appdata directory dall'altra parte costa di piu, qui ci vuole una riga
            //dbStream devo calcolarlo qua, perchè la risorsa che è embeddata in maui la vedo solo da qua
            await DbConnection.EmbeddedDbUpload(mauiAppName, dbStream, dbPath, dbFileName!);
        }

        /// <summary>
        /// recupera il db embeddato, ATTENZIONE, se da errore qui dentro probabilmente non si è messo nelle proprietà del file del db il fatto che deve
        /// venire caricato come risorsa embedded
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="mauiAsm"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        private static  Stream getEmbeddedDatabaseStream(string resourceName, Assembly mauiAsm)
        {
            var s = mauiAsm.GetManifestResourceStream(resourceName);
            if (s is null)
                throw new FileNotFoundException(
                    $"Embedded resource '{resourceName}' not found in {mauiAsm.GetName().Name}");
            return s;
        }
    }
}
