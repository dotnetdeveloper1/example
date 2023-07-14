using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class SymmetricSigningCredentialStore : ISigningCredentialStore, IValidationKeysStore
    {
        public SymmetricSigningCredentialStore(IOptions<AuthenticationServerOptions> optionsAccessor)
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(optionsAccessor.Value, nameof(optionsAccessor.Value));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.SigningKey, nameof(optionsAccessor.Value.SigningKey));

            options = optionsAccessor.Value;

            key = CreateSecurityKey(options.SigningKey);
            algorithm = SecurityAlgorithms.HmacSha512;
        }

        public Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var signingCredentials = new SigningCredentials(key, algorithm);

            return Task.FromResult(signingCredentials);
        }

        public Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            var validationKeys = new List<SecurityKeyInfo> 
            {
                new SecurityKeyInfo { Key = key, SigningAlgorithm = algorithm } 
            };

            return Task.FromResult<IEnumerable<SecurityKeyInfo>>(validationKeys);
        }

        private SecurityKey CreateSecurityKey(string signingKey) 
        {
            var keyBinary = Encoding.ASCII.GetBytes(signingKey);

            return new SymmetricSecurityKey(keyBinary);
        }

        private readonly SecurityKey key;
        private readonly string algorithm;
        private readonly AuthenticationServerOptions options;
    }
}
