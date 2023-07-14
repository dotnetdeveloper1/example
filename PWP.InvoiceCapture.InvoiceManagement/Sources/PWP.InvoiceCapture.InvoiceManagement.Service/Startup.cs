using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.DataAccess.Extensions;
using PWP.InvoiceCapture.Core.Extensions;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Document.API.Client.Extensions;
using PWP.InvoiceCapture.Document.API.Client.Options;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.InvoiceManagement.Service.MessageHandlers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service
{
    public class Startup : ServiceStartupBase
    {
        public override void ConfigureServices(IServiceCollection services) 
        {
            base.ConfigureServices(services);

            services.AddDocumentApiClient();
            services.AddSerializationService();
            services.AddInvoicesDatabaseNameProvider();

            AddMessageHandlers(services);
            AddInvoiceManagementServiceBusPublisher(services);
            StartInvoiceManagementServicBusSubscriber(services);
        }

        protected override void ConfigureOptions(IServiceCollection services)
        {
            base.ConfigureOptions(services);
            
            services.Configure<DocumentApiClientOptions>(configuration.GetSection("DocumentApiClientOptions"));
            services.Configure<ImageConversionOptions>(configuration.GetSection("ImageConversionOptions"));
            services.Configure<DatabaseOptions>(configuration.GetSection("InvoicesDatabaseOptions"));
            services.Configure<NotificationOptions>(configuration.GetSection("NotificationOptions"));
        }

        public override async Task OnShutdownAsync(CancellationToken cancellationToken)
        {
            await base.OnShutdownAsync(cancellationToken);
            await subscriber?.StopAsync(cancellationToken);
            await publisher?.StopAsync(cancellationToken);
        }

        protected override ICompositionModule[] GetCompositionModules()
        {
            return new ICompositionModule[]
            {
                new Business.CompositionModule.CompositionModule(),
                new DataAccess.CompositionModule.CompositionModule()
            };
        }

        private void AddMessageHandlers(IServiceCollection services)
        {
            services.AddSingleton<IMessageHandler, InvoiceProcessingLimitExceededMessageHandler>();
            services.AddSingleton<IMessageHandler, InvoiceProcessingLimitNotExceededMessageHandler>();
            services.AddSingleton<IMessageHandler, InvoiceDocumentUploadedMessageHandler>();
            services.AddSingleton<IMessageHandler, InvoiceRecognitionCompletedHandler>();
            services.AddSingleton<IMessageHandler, InvoiceProcessingStartedMessageHandler>();
            services.AddSingleton<IMessageHandler, InvoiceDataAnalysisCompletedMessageHandler>();
            services.AddSingleton<IMessageHandler, InvoiceProcessingErrorMessageHandler>();
            services.AddSingleton<IMessageHandler, InvoiceStatusChangedMessageHandler>();
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

        private void StartInvoiceManagementServicBusSubscriber(IServiceCollection services) 
        {
            var factory = new ServiceBusSubscriberFactory();
            var options = new ServiceBusSubscriberOptions();

            configuration
                .GetSection("InvoiceManagementServiceBusSubscriberOptions")
                .Bind(options);

            var serviceProvider = services.BuildServiceProvider();

            var telemetryClient = serviceProvider.GetService<ITelemetryClient>();
            var exceptionHandler = new DefaultExceptionHandler(telemetryClient);
            
            var messageHandlers = serviceProvider
                .GetServices<IMessageHandler>()
                .ToList();

            subscriber = factory.Create(messageHandlers, exceptionHandler, options);
            subscriber.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        private IServiceBusPublisher publisher;
        private IServiceBusSubscriber subscriber;
    }
}
