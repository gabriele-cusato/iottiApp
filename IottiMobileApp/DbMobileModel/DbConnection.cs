using System.Security.Cryptography;
using DbMobileModel.Context;
using DbMobileModel.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DbMobileModel.Services.Interfaces;

namespace DbMobileModel
{
    public class DbConnection
    {
        //ricorda di aggiornare il pacchetto dotnet ef prima: 
        //dotnet tool update --global dotnet-ef

        //DATABASE MOBILE (file del db nel progetto principale)
        //dotnet ef dbcontext scaffold "Data Source='C:\Projects\IOTTI_APP\IottiMobileApp\IottiMobileApp\Resources\MobileDb\DbIOT.db'" Microsoft.EntityFrameworkCore.Sqlite -o Models/LocalDb --context-dir Context --context LocalDbContext --no-pluralize --force

        //DATABASE INTERMEDIO:
        //dotnet ef dbcontext scaffold "Server=192.168.184.130,1433;Database=DbIOT;User Id=sa;Password=aA1bB2cC3dD4eE5;Encrypt=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models/IntermediateDb --context-dir Context --context IntermediateDbContext --no-pluralize --force

        public static IServiceProvider? serviceProvider;

        /// <summary>
        /// Verifica se il db già sul dispositivo esiste e se è vuoto, 
        /// se è vuoto allora controllo se il db embeddato che voglio caricare ha dei contenuti e in tal caso 
        /// viene caricato nella cartella AppDataDiretory dell'applicazione
        /// </summary>
        /// <param name="mauiName"></param>
        /// <param name="ResourceUploadDb"></param>
        /// <param name="dbPath"></param>
        /// <param name="dbFileName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static async Task EmbeddedDbUpload(string mauiName, Stream ResourceUploadDb, string dbPath, string dbFileName)
        {
            ILocalDbService localDbService;

            //questa variabile è stat valorizzata nel mauiProgram
            if (serviceProvider != null)
                localDbService = serviceProvider.GetRequiredService<ILocalDbService>();
            else
                throw new System.Data.DataException($"Database remoto non raggiungibile o non trovato.");

            // 2) Chiamalo
            bool isEmpty = await localDbService.IsLocalDbEmptyAsync(dbPath);

            //isEmpty verifica già se il db esiste, se non esiste non entra nell'if
            if (isEmpty && !DatabaseIsIdentical(dbPath, ResourceUploadDb) )
            {
                await CopyDatabaseFromResourcesAsync(dbFileName, dbPath, ResourceUploadDb);
            }
        }

        private static async Task CopyDatabaseFromResourcesAsync(string dbFileName, string destinationPath, Stream resourceStream)
        {
            // Copia il contenuto della risorsa nel file di destinazione
            using FileStream fileStream = File.Create(destinationPath);
            await resourceStream.CopyToAsync(fileStream);
        }

        //Metodo per verificare se il database locale è identico a quello in Resources
        private static bool DatabaseIsIdentical(string dbPath, Stream resourceStream)
        {
            if (!File.Exists(dbPath)) return false;

            using var dbStream = File.OpenRead(dbPath);

            string localHash = ComputeHash(dbStream);
            string resourceHash = ComputeHash(resourceStream);

            // Reset dello stream dopo il calcolo dell'hash per uso successivo
            resourceStream.Position = 0;

            return localHash == resourceHash;
        }

        //Metodo per calcolare l'hash di un file
        private static string ComputeHash(Stream stream)
        {
            using var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}