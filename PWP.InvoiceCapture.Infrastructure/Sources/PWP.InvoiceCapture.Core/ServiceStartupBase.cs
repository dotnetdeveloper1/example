using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.Extensions;
using PWP.InvoiceCapture.Core.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core
{
    public abstract class ServiceStartupBase : IServiceStartup
    {
        public virtual void ConfigureHostConfiguration(IConfigurationBuilder builder)
        {
            Guard.IsNotNull(builder, nameof(builder));

            builder.AddEnvironmentVariables();
        }

        public virtual void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        {
            Guard.IsNotNull(context, nameof(context));
            Guard.IsNotNull(builder, nameof(builder));

            builder
                .SetBasePath(context.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);

            configuration = builder.Build();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            Guard.IsNotNull(services, nameof(services));

            services.AddOptions();
            services.AddApplicationInsightsTelemetryWorkerService();
            services.AddTelemetryClient();
            services.AddBackgroundServiceContext();

            ConfigureOptions(services);
            ConfigureModules(services);
        }

        public virtual Task OnShutdownAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual void ConfigureOptions(IServiceCollection services)
        { }

        protected abstract ICompositionModule[] GetCompositionModules();

        protected IConfiguration configuration;

        private void ConfigureModules(IServiceCollection services)
        {
            var modules = GetCompositionModules();

            foreach (var module in modules)
            {
                module.RegisterTypes(services);
            }
        }
    }
}
