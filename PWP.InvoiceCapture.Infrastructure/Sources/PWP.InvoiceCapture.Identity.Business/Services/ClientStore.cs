using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Definitions;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class ClientStore : IClientStore
    {
        public ClientStore(IOptions<AuthenticationServerOptions> optionsAccessor, IApplicationClientService applicationClientService) 
        {
            GuardOptions(optionsAccessor);
            Guard.IsNotNull(applicationClientService, nameof(applicationClientService));

            options = optionsAccessor.Value;
            this.applicationClientService = applicationClientService;
        }
        
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return null;
            }    

            var applicationClient = await applicationClientService.GetAsync(clientId, CancellationToken.None);
            if (applicationClient == null)
            {
                return null;
            }

            //TODO: Move GrantTypes and Scopes to Db
            switch (applicationClient.ClientId)
            {
                case "defaultClient":
                    return CreateClient(clientId, applicationClient.SecretHash, true, GrantTypes.ResourceOwnerPasswordAndClientCredentials, new string[] { Scopes.OcrDataAnalysis, Scopes.TenantManagement, Scopes.InvoiceManagement });
                case "webApplication":
                    return CreateClient(clientId, applicationClient.SecretHash, false, GrantTypes.ResourceOwnerPassword, new string[] { Scopes.OcrDataAnalysis, Scopes.InvoiceManagement });
                default:
                    return null;
            }
        }

        private Client CreateClient(string clientId, string passwordHash, bool requireClientSecret, ICollection<string> allowedGrantTypes, ICollection<string> allowedScopes) 
        {
            var scopes = allowedScopes.ToList();

            scopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            scopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);

            return new Client
            {
                ClientId = clientId,
                Enabled = true,
                ClientSecrets = new[] { new Secret(passwordHash) },
                AllowedGrantTypes = allowedGrantTypes,
                AllowedScopes = scopes,
                AccessTokenType = AccessTokenType.Jwt,
                AccessTokenLifetime = options.AccessTokenLifetimeInSeconds,
                AllowOfflineAccess = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Absolute,
                AbsoluteRefreshTokenLifetime = options.RefreshTokenLifetimeInSeconds,
                UpdateAccessTokenClaimsOnRefresh = true,
                RequireConsent = false,
                RequireClientSecret = requireClientSecret,
            };
        }

        private void GuardOptions(IOptions<AuthenticationServerOptions> optionsAccessor)
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(optionsAccessor.Value, nameof(optionsAccessor.Value));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.SigningKey, nameof(optionsAccessor.Value.SigningKey));
            Guard.IsNotZeroOrNegative(optionsAccessor.Value.RefreshTokenLifetimeInSeconds, nameof(optionsAccessor.Value.RefreshTokenLifetimeInSeconds));
            Guard.IsNotZeroOrNegative(optionsAccessor.Value.AccessTokenLifetimeInSeconds, nameof(optionsAccessor.Value.AccessTokenLifetimeInSeconds));
        }

        private readonly AuthenticationServerOptions options;
        private readonly IApplicationClientService applicationClientService;
    }
}
