using DbMobileModel.Context;
using DbMobileModel.Models.IntermediateDb;
using DbMobileModel.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DbMobileModel.Services
{
    public class IntermediateDbService : IIntermediateDbService
    {
        private readonly IntermediateDbContext _context;

        //fornire un’interfaccia per DbContext crea problemi nel far evolvere l’API in futuro e non è una scelta consigliata, quindi i context vanno lasciati come classi
        public IntermediateDbService(IntermediateDbContext context)
        {
            _context = context;
        }

        // Metodo per ottenere tutte le merci
        //public async Task<IEnumerable<Articoli>> GetAllMerceAsync()
        //{
        //    return await _context.Articoli.ToListAsync();
        //}

        //// Metodo per aggiungere una nuova merce
        //public async Task AddMerceAsync(Articoli merce)
        //{
        //    if (merce == null)
        //        throw new ArgumentNullException(nameof(merce));

        //    _context.Articoli.Add(merce);
        //    await _context.SaveChangesAsync();
        //}

        public async Task<Utente?> GetUtenteByUsernameAsync(string username)
        {
            if (_context == null) return null;
            return await _context.Utente
                .FirstOrDefaultAsync(u => u.UtnUsername == username);
        }

        public async Task<List<MissioneTes>?> GetAllFiereAsync()
        {
            if (_context == null) return null;
            return await _context.MissioneTes.ToListAsync().ConfigureAwait(false);
        }
    }
}
