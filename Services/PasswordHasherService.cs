using System.Security.Cryptography;
using KURS_ASP.NET.Services.Interfaces;

namespace KURS_ASP.NET.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public string HashPassword(string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return string.Join('.', Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            var parts = passwordHash.Split('.', 3);
            if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            {
                return false;
            }

            try
            {
                var salt = Convert.FromBase64String(parts[1]);
                var expectedKey = Convert.FromBase64String(parts[2]);
                var actualKey = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedKey.Length);

                return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
