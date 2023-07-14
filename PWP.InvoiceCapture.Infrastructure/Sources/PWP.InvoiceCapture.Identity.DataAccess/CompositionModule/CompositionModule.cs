using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;

namespace PWP.InvoiceCapture.Identity.DataAccess.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer();
            services.AddSingleton<ITenantsDatabaseContextFactory, TenantsDatabaseContextFactory>();
            services.AddSingleton<IMasterDatabaseContextFactory, MasterDatabaseContextFactory>();
            services.AddTransient<ITenantRepository, TenantRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IPlanRepository, PlanRepository>();
            services.AddTransient<IPackRepository, PackRepository>();
            services.AddTransient<IGroupPackRepository, GroupPackRepository>();
            services.AddTransient<IGroupPlanRepository, GroupPlanRepository>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<ISqlDatabaseRepository, SqlDatabaseRepository>();
            services.AddTransient<IApplicationClientRepository, ApplicationClientRepository>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<ITenantSettingRepository, TenantSettingRepository>();
            services.AddTransient<ICultureRepository, CultureRepository>();
            services.AddTransient<ICurrencyRepository, CurrencyRepository>();
            services.AddTransient<IPersistedGrantRepository, PersistedGrantRepository>();
        }
    }
}
