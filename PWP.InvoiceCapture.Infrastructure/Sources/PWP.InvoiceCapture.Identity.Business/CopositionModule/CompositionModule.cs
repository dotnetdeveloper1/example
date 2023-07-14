using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.Identity.Business.Services;

namespace PWP.InvoiceCapture.Identity.Business.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddSingleton<IClientStore, ClientStore>();
            services.AddSingleton<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            services.AddSingleton<ISigningCredentialStore, SymmetricSigningCredentialStore>();
            services.AddSingleton<IValidationKeysStore, SymmetricSigningCredentialStore>();
            services.AddTransient<ISqlManagementService, SqlManagementService>();
            services.AddTransient<IPasswordHashService, PasswordHashService>();
            services.AddTransient<ITenantService, TenantService>();
            services.AddTransient<IPlanService, PlanService>();
            services.AddTransient<IPackService, PackService>();
            services.AddTransient<IUsageService, UsageService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPasswordGenerator, PasswordGenerator>();
            services.AddTransient<INameGenerator, UniqueNameGenerator>();
            services.AddTransient<IApplicationClientService, ApplicationClientService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<ICultureService, CultureService>();
            services.AddTransient<ITenantSettingService, TenantSettingService>();
            services.AddTransient<IPersistedGrantStore, PersistedGrantService>();
            services.AddTransient<IPersistedGrantService, PersistedGrantService>();
            services.AddTransient<IEmailProvider, EmailProvider>();
            services.AddTransient<IPlanRenewalService, PlanRenewalService>();
        }
    }
}
