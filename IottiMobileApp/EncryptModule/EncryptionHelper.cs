using BC = BCrypt.Net.BCrypt;

namespace EncryptModule
{
    public class EncryptionHelper : IPasswordHasher
    {
        private readonly int _workFactor;

        public EncryptionHelper(int workFactor = 12)
        {
            _workFactor = workFactor;
        }
        public string HashPassword(string password)
        {
            // Genera automaticamente salt + cost factor incorporato nell’hash
            return BC.HashPassword(password, _workFactor);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            // Estrae salt e cost factor dall’hash e verifica in modo sicuro
            return BC.Verify(password, passwordHash);
        }
    }
}
