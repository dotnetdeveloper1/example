using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Linq;
using System.Text;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class PasswordHashService: IPasswordHashService
    {
        public string GetHash(string password)
        {
            Guard.IsNotNullOrWhiteSpace(password, nameof(password));

            var salt = GetSalt(password);
            var hash = KeyDerivation.Pbkdf2(password, salt, keyDerivationFunction, iterationCount, hashSizeBytes);

            return Convert.ToBase64String(hash);
        }

        private byte[] GetSalt(string password)
        {
            var salt = new byte[saltSizeBytes];
            var passwordBinary = Encoding.UTF8.GetBytes(password);
            var seed = password.Length + Convert.ToInt32(passwordBinary.First());
            var random = new Random(seed);

            random.NextBytes(salt);

            return salt;
        }

        // We want our hashing algorithm to be a bit slow (not to much not to affect UX) to add a delay to the attacker
        private const int iterationCount = 10000;
        private const int saltSizeBytes = 16;
        private const int hashSizeBytes = 64;
        private const KeyDerivationPrf keyDerivationFunction = KeyDerivationPrf.HMACSHA512;
    }
}
