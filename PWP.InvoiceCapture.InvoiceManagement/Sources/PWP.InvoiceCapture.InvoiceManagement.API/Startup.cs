using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.API.Extensions;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.DataAccess.Extensions;
using PWP.InvoiceCapture.Core.Extensions;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Document.API.Client.Extensions;
using PWP.InvoiceCapture.Document.API.Client.Options;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using System.Threading;
using StartupBase = PWP.InvoiceCapture.Core.API.StartupBase;

namespace PWP.InvoiceCapture.InvoiceManagement.API
{
    public class Startup : StartupBase
    {
        public Startup(IWebHostEnvironment environment) : base(environment)
        { }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddDocumentApiClient();
            services.AddSerializationService();
            services.AddInvoicesDatabaseNameProvider();
            services.AddApiVersioning(0, 9);

            AddInvoiceManagementServiceBusPublisher(services);
        }

        protected override void ConfigureOptions(IServiceCollection services)
        {
            base.ConfigureOptions(services);

            services.Configure<DatabaseOptions>(configuration.GetSection("InvoicesDatabaseOptions"));
            services.Configure<DocumentApiClientOptions>(configuration.GetSection("DocumentApiClientOptions"));
        }

        protected override ICompositionModule[] GetCompositionModules()
        {
            return new ICompositionModule[]
            {
                new Business.CompositionModule.CompositionModule(),
                new DataAccess.CompositionModule.CompositionModule()
            };
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();

            publisher?.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        private void AddInvoiceManagementServiceBusPublisher(IServiceCollection services)
        {
            var factory = new ServiceBusPublisherFactory();
            var options = new ServiceBusPublisherOptions();

            configuration
                .GetSection("InvoiceManagementServiceBusPublisherOptions")
                .Bind(options);

            publisher = factory.Create(options);
            publisher.StartAsync(CancellationToken.None).GetAwaiter().GetResult();

            services.AddSingleton(publisher);
        }

        protected override string serviceName => "PWP.InvoiceCapture.InvoiceManagement.API";

        protected override string apiVersion => "v1";
        
        private IServiceBusPublisher publisher;
    }
}
