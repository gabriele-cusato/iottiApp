using DbMobileModel.DTO;
using DbMobileModel.Models.IntermediateDb;

namespace DbMobileModel.Services.Interfaces
{
    public interface IRemoteAuthService
    {
        //Task<bool> RegisterAsync(RegisterDto request);
        Task<Utente?> LoginAsync(LoginDto request);
    }
}
