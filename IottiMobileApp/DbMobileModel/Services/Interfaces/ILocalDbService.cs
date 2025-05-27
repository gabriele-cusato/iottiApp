using DbMobileModel.Models.LocalDb;

namespace DbMobileModel.Services.Interfaces
{
    public interface ILocalDbService
    {
        /// <summary>
        /// Verifica se il database è vuoto.
        /// </summary>
        Task<bool> IsLocalDbEmptyAsync(string dbPath);

        /// <summary>
        /// Ottiene tutte le merci.
        /// </summary>
        //Task<IEnumerable<Articoli>> GetAllMerceAsync();

        /// <summary>
        /// Ottiene tutte le missioni.
        /// </summary>
        //Task<IEnumerable<Missioni>> GetAllMissioniAsync();

        /// <summary>
        /// Aggiunge una nuova merce.
        /// </summary>
        //Task AddMerceAsync(Articoli merce);
    }
}
