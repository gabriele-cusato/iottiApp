using System.Reflection;
using DbMobileModel.Context;
using DbMobileModel.Models.LocalDb;
using DbMobileModel.Services.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DbMobileModel.Services
{
    public class LocalDbService : ILocalDbService
    {
        private readonly LocalDbContext _context;

        //fornire un’interfaccia per DbContext crea problemi nel far evolvere l’API in futuro e non è una scelta consigliata, quindi i context vanno lasciati come classi
        public LocalDbService(LocalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Restituisce true se entrambe le tabelle Articoli e Missioni sono vuote.
        /// Al primo elemento trovato in una di esse, ritorna false.
        /// </summary>
        public async Task<bool> IsLocalDbEmptyAsync(string dbPath)
        {
            // 1) Se il file non esiste, è “vuoto” per definizione
            if (!File.Exists(dbPath))
                return true;

            // 2) Crea connessione e aprila
            var cs = $"Data Source={dbPath}";
            await using var conn = new SqliteConnection(cs);
            await conn.OpenAsync();

            // 3) Estrai dinamicamente i nomi delle tabelle utente
            var tables = new List<string>();
            await using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                  @"SELECT name 
                    FROM sqlite_master 
                    WHERE type='table' 
                    AND name NOT LIKE 'sqlite_%';";
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    tables.Add(reader.GetString(0));
            }

            // 4) Per ogni tabella, conta le righe e ferma al primo >0
            foreach (var table in tables)
            {
                await using var countCmd = conn.CreateCommand();
                countCmd.CommandText = $"SELECT COUNT(*) FROM \"{table}\";";
                var result = await countCmd.ExecuteScalarAsync();
                if (result is long l && l > 0)
                    return false;  // trovata una riga -> DB non è vuoto
            }

            // 5) Se nessuna tabella contiene righe, restituisci true
            return true;
        }

        // Metodo per ottenere tutte le merci
        //public async Task<IEnumerable<Articoli>> GetAllMerceAsync()
        //{
        //    return await _context.Articoli.ToListAsync();
        //}

        //public async Task<IEnumerable<Missioni>> GetAllMissioniAsync()
        //{
        //    return await _context.Missioni.ToListAsync();
        //}

        //// Metodo per aggiungere una nuova merce
        //public async Task AddMerceAsync(Articoli merce)
        //{
        //    if (merce == null)
        //        throw new ArgumentNullException(nameof(merce));

        //    _context.Articoli.Add(merce);
        //    await _context.SaveChangesAsync();
        //}
    }
}
