using DbMobileModel.Models.IntermediateDb;
using Microsoft.EntityFrameworkCore;

namespace DbMobileModel.Services.Interfaces
{
    public interface IIntermediateDbService
    {
        /// <summary>
        /// Ottiene tutte le merci.
        /// </summary>
        //Task<IEnumerable<Articoli>> GetAllMerceAsync();

        /// <summary>
        /// Aggiunge una nuova merce.
        /// </summary>
        //Task AddMerceAsync(Articoli merce);

        public Task<Utente?>        GetUtenteByUsernameAsync(string username);
        public Task<List<MissioneTes>?> GetAllFiereAsync();
    }
}
