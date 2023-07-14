using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.DataAccess.Extensions;
using PWP.InvoiceCapture.Identity.Business.Contract.Configurations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using StartupBase = PWP.InvoiceCapture.Core.API.StartupBase;

namespace PWP.InvoiceCapture.Identity.API
{
    public class Startup : StartupBase
    {
        public Startup(IWebHostEnvironment environment) : base(environment)
        { }

        public override void ConfigureServices(IServiceCollection services) 
        {
            base.ConfigureServices(services);

            services
                .AddIdentityServer()
                .AddInMemoryApiScopes(ScopesConfiguration.ApiScopes)
                .AddInMemoryApiResources(ApiResourcesConfiguration.ApiResources);

            services.AddInvoicesDatabaseNameProvider();
        }

        public override void Configure(IApplicationBuilder application, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment environment) 
        {
            base.Configure(application, applicationLifetime, environment);

            application.UseIdentityServer();
        }

        protected override void ConfigureOptions(IServiceCollection services) 
        {
            base.ConfigureOptions(services);

            services.Configure<AuthenticationServerOptions>(configuration.GetSection("AuthenticationServerOptions"));
            services.Configure<DatabaseOptions>(configuration.GetSection("TenantsDatabaseOptions"));
            services.Configure<SqlManagementClientOptions>(configuration.GetSection("SqlManagementClientOptions"));
            services.Configure<EmailAddressGenerationOptions>(configuration.GetSection("EmailAddressGenerationOptions"));
            services.Configure<LongTermSqlServerBackupOptions>(configuration.GetSection("LongTermSqlServerBackupOptions"));
        }

        protected override ICompositionModule[] GetCompositionModules()
        {
            return new ICompositionModule[]
            {
                new Business.CompositionModule.CompositionModule(),
                new DataAccess.CompositionModule.CompositionModule()
            };
        }

        protected override string serviceName => "PWP.InvoiceCapture.Identity.API";
        protected override string apiVersion => "v1";
    }
}
