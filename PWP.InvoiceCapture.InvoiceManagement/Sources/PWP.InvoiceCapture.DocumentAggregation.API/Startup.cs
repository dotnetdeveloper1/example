using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.Extensions;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Document.API.Client.Extensions;
using PWP.InvoiceCapture.Document.API.Client.Options;
using PWP.InvoiceCapture.DocumentAggregation.API.Middlewares;
using System.Threading;
using StartupBase = PWP.InvoiceCapture.Core.API.StartupBase;

namespace PWP.InvoiceCapture.DocumentAggregation.API
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
            services.AddTelemetryClient();

            AddInvoiceManagementServiceBusPublisher(services);
        }

        public override void Configure(IApplicationBuilder application, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment environment)
        {
            base.Configure(application, applicationLifetime, environment);

            application.Map("/emaildocuments", app => app.UseMiddleware<EmailDocumentUploadMiddleware>());
        }

        protected override void ConfigureOptions(IServiceCollection services) 
        {
            base.ConfigureOptions(services);

            services.Configure<DocumentApiClientOptions>(configuration.GetSection("DocumentApiClientOptions"));
        }

        protected override ICompositionModule[] GetCompositionModules() 
        {
            return new ICompositionModule[] 
            {
                new Business.CompositionModule.CompositionModule()
            };
        }

        //TODO: 8751 - Tech-Debt: Make sure API projects can shutdown gracefully
        protected override void OnShutdown() 
        {
            base.OnShutdown();

            publisher?.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        protected override string serviceName => "PWP.InvoiceCapture.DocumentAggregation.API";

        protected override string apiVersion => "v1";

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

        private IServiceBusPublisher publisher;
    }
}
