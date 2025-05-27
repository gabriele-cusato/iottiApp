using DbMobileModel.Context;
using DbMobileModel.Models.IntermediateDb;
using Microsoft.EntityFrameworkCore;
using EncryptModule;
using DbMobileModel.Services.Interfaces;
using DbMobileModel.DTO;

namespace DbMobileModel.Services
{
    public class RemoteAuthService : IRemoteAuthService
    {
        /*
            Il BcryptWorkFactor (viene chiamato cost) determina il numero di iterazioni (2^cost), rendendo bcrypt adattivo nel tempo: un valore tipico è 10–12 per bilanciare sicurezza e prestazioni 
            La libreria BCrypt.Net-Next è un port .NET di OpenBSD bcrypt basato su Blowfish; basta installare il pacchetto NuGet per accedere alle API statiche BCrypt.GenerateSalt(), HashPassword() e Verify()
        */

        private readonly IntermediateDbContext _context;
        private readonly IPasswordHasher _hasher;

        //fornire un’interfaccia per DbContext crea problemi nel far evolvere l’API in futuro e non è una scelta consigliata, quindi i context vanno lasciati come classi
        public RemoteAuthService(IntermediateDbContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        /// <summary>
        /// Registra un nuovo utente: verifica unicità username,
        /// delega l’hashing a BCryptPasswordHasher e salva l’entità.
        /// </summary>
        //public async Task<bool> RegisterAsync(RegisterDto request)
        //{
        //    // 1) Controllo unicità username o email
        //    if (await _context.Utente.AnyAsync(u =>
        //            u.UtnEmail == request.email || u.UtnEmail == request.email))
        //        return false;

        //    // 2) Hash della password
        //    string hash = _hasher.HashPassword(request.password);

        //    // 3) Creazione e salvataggio entità
        //    var user = new Utente
        //    {
        //        UtnEmail = request.email,
        //        UtnPassword = hash,

        //    };
        //    await _context.Utente.AddAsync(user);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        /// <summary>
        /// Login: recupera l’utente e delega la verifica della password
        /// (bcrypt estrae salt e cost automaticamente).
        /// </summary>
        public async Task<Utente?> LoginAsync(LoginDto request)
        {
            // 1) Caricamento utente da DB
            var user = await _context.Utente
                .SingleOrDefaultAsync(u => u.UtnUsername == request.username);     
            if (user == null)
                return null;

            // 2) Verifica password cifrata
            //return _hasher.VerifyPassword(request.password, user.PasswordHash);

            //verifica password in chiaro
            if (request.password == user.UtnPassword)
                return user;
            else
                return null;
        }
    }
}
