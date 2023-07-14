using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.Extensions;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Document.API.Client.Extensions;
using PWP.InvoiceCapture.Document.API.Client.Options;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.OCR.Core;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Extensions;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Options;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Mappers;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Options;
using PWP.InvoiceCapture.OCR.Recognition.Business.Mapper;
using PWP.InvoiceCapture.OCR.Recognition.Service.MessageHandlers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Service
{
    public class Startup : ServiceStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddDocumentApiClient();
            services.AddFormRecognizerClient();
            services.AddInvoiceTemplates();

            services.AddSerializationService();
            AddMessageHandlers(services);
            AddInvoiceManagementServiceBusPublisher(services);
            StartInvoiceManagementServicBusSubscriber(services);
        }

        public override async Task OnShutdownAsync(CancellationToken cancellationToken)
        {
            await base.OnShutdownAsync(cancellationToken);
            await subscriber?.StopAsync(cancellationToken);
            await publisher?.StopAsync(cancellationToken);
        }

        protected override void ConfigureOptions(IServiceCollection services)
        {
            base.ConfigureOptions(services);

            services.Configure<DocumentApiClientOptions>(configuration.GetSection("DocumentApiClientOptions"));
            services.Configure<FormRecognizerClientPoolOptions>(configuration.GetSection("FormRecognizerClientPoolOptions"));
            services.Configure<DatabaseOptions>(configuration.GetSection("InvoiceTemplateDatabaseOptions"));
            services.Configure<RecognitionDatabaseOptions>(configuration.GetSection("InvoiceTemplateDatabaseOptions"));

            services.Configure<DocumentStorageOptions>(configuration.GetSection("TrainingBlobRepositoryOptions"));
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
            services.AddSingleton<IFieldMapper, FieldMapper>();
            services.AddSingleton<IMessageHandler, InvoiceReadyForRecognitionMessageHandler>();
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
