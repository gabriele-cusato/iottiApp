using Utils.Classes;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IottiMobileApp.Classes
{
    internal class ConfigParams : ConfigObjBase
    {
        // Parametri letti dal file di configurazione
        public string SettingsFileName { get; set; } = string.Empty;
        public string EmbeddedDbFolder { get; set; } = string.Empty;
        public bool Debug { get; set; }
        public int Count { get; set; }
        public float CountFloat { get; set; }

        public string LocalServerConnection { get; set; } = string.Empty;
        public string IntermediateServerConnection { get; set; } = string.Empty;

        /// <summary>
        /// variabile utilizzata per tenere traccia se effettivamente è stato passato il file di config nello stream come embeddedResource
        /// se è false lo prendo dall'appdata directory
        /// </summary>
        public bool fileFromResource { get; set; }

        /// <summary>
        /// Costruttore della classe configParams, va a popolare i parametri di configurazione prendendo i valori dal file di config
        /// prima controlla se c'è il file di configurazione nelle risorse embeddate nella root del progetto, se non c'è controlla nella cartella
        /// AppData dell'applicazione
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="FileNotFoundException"></exception>
        internal ConfigParams(string fileName)
        {
            //questo mi permette di costruire la variabile di configurazione
            IConfigurationRoot configuration;

            //cerco di prendere il file di configurazione dalle risorse embedded 
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream($"IottiMobileApp.{fileName}");

            if (stream != null)
            {
                //se ho trovato il file nelle risorse ne prendo il contenuto
                configuration = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();

                fileFromResource = true;
            }
            else
            {
                //se il file non era nelle risorse embedded lo cerco nella cartella appData dell'applicazione
                string appDataPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                //se non lo trovo nemmeno li lancio un eccezione
                if (!File.Exists(appDataPath))
                {
                    throw new FileNotFoundException(
                        $"File di configurazione non trovato né come EmbeddedResource né in AppDataDirectory.\nPercorso atteso: {appDataPath}");
                }

                //se ho trovato il file nella cartella appData carico il contenuto
                configuration = new ConfigurationBuilder()
                    .AddJsonFile(appDataPath)
                    .Build();

                fileFromResource = false;
            }

            //eseguo il binding tra le proprietà (che in realtà sono sezioni) dell'oggetto configuration, alle proprietà di ConfigParams
            //per renderle utilizzabili dall'eseterno
            configuration.GetSection("ConnectionStrings").Bind(this);
            configuration.GetSection("ConfigParams").Bind(this);
        }
    }
}
