using IdentityServer4.Models;
using IdentityServer4.Validation;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Definitions;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public ResourceOwnerPasswordValidator(IPasswordHashService passwordHashService,
            IUserService userService,
            ITenantService tenantService,
            ITenantSettingService tenantSettingService,
            ICultureService cultureService)
        {
            Guard.IsNotNull(passwordHashService, nameof(passwordHashService));
            Guard.IsNotNull(userService, nameof(userService));
            Guard.IsNotNull(tenantService, nameof(tenantService));
            Guard.IsNotNull(tenantSettingService, nameof(tenantSettingService));
            Guard.IsNotNull(cultureService, nameof(cultureService));

            this.passwordHashService = passwordHashService;
            this.userService = userService;
            this.tenantService = tenantService;
            this.tenantSettingService = tenantSettingService;
            this.cultureService = cultureService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UserName) || string.IsNullOrWhiteSpace(context.Password))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, invalidUsernameOrPasswordMessage);
                return;
            }

            var user = await userService.GetAsync(context.UserName, CancellationToken.None);

            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, invalidUsernameOrPasswordMessage);
                return;
            }

            var passwordHash = passwordHashService.GetHash(context.Password);

            if (!string.Equals(passwordHash, user.PasswordHash))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, invalidUsernameOrPasswordMessage);
                return;
            }

            var tenant = await GetTenantAsync(context);

            if (tenant == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, invalidTenantIdMessage);
                return;
            }

            if (tenant.GroupId != user.GroupId)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, differentGroupIdMessage);
                return;
            }

            var tenantSetting = await tenantSettingService.GetAsync(tenant.Id, CancellationToken.None);
            var cultureName = "en-US";

            if (tenantSetting != null)
            {
                var culture = await cultureService.GetAsync(tenantSetting.CultureId, CancellationToken.None);
                if (culture != null)
                {
                    cultureName = culture.Name;
                }
            }

            var tenantId = GetTenantIdName(tenant);

            var claims = new List<Claim>
            {
                new Claim(InvoiceManagementClaims.TenantId, tenantId),
                new Claim(InvoiceManagementClaims.Culture, cultureName)
            };

            context.Result = new GrantValidationResult(tenantId, "password", claims);
        }

        private async Task<Tenant> GetTenantAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (context.Request == null || context.Request.Raw == null)
            {
                return null;
            }

            var key = context.Request.Raw.AllKeys.FirstOrDefault(requestKey => string.Equals(requestKey, tenantIdKey, StringComparison.OrdinalIgnoreCase));

            if (key == null)
            {
                return null;
            }

            var requestTenantId = context.Request.Raw[key];

            int.TryParse(requestTenantId, out int parsedTenantId);
            
            return await tenantService.GetAsync(parsedTenantId, CancellationToken.None);            
        }

        //TODO: Refactor to use 1 instead of Default in the whole solution, or move this logic to common DatabaseNameProvider (Core project)
        private string GetTenantIdName(Tenant tenant)
        {
            if (string.Equals(tenant.DatabaseName, "Invoices_Default"))
            {
                //For developer database only.
                return "Default";
            }

            return tenant.Id.ToString();
        }

        private readonly IPasswordHashService passwordHashService;
        private readonly IUserService userService;
        private readonly ITenantService tenantService;
        private readonly ITenantSettingService tenantSettingService;
        private readonly ICultureService cultureService;

        private const string invalidUsernameOrPasswordMessage = "Invalid Username or Password.";
        private const string invalidTenantIdMessage = "Invalid tenant id.";
        private const string differentGroupIdMessage = "User and Tenant belongs to different groups.";
        private const string tenantIdKey = "tenantId";
    }
}
