using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.Extensions;
using PWP.InvoiceCapture.Document.API.Client.Extensions;
using PWP.InvoiceCapture.Document.API.Client.Options;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.OCR.Core;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Extensions;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Options;
using PWP.InvoiceCapture.OCR.Core.Models;
using StartupBase = PWP.InvoiceCapture.Core.API.StartupBase;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.API
{
    public class Startup: StartupBase
    {
        public Startup(IWebHostEnvironment environment) : base(environment)
        { }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddTelemetryClient();
            services.AddDocumentApiClient();
            services.AddSerializationService();
            services.AddFormRecognizerClient();
            services.AddInvoiceTemplates();
        }

        protected override void ConfigureOptions(IServiceCollection services)
        {
            base.ConfigureOptions(services);

            services.Configure<DocumentApiClientOptions>(configuration.GetSection("DocumentApiClientOptions"));
            services.Configure<FormRecognizerClientPoolOptions>(configuration.GetSection("FormRecognizerClientPoolOptions"));
            services.Configure<DocumentStorageOptions>(configuration.GetSection("TrainingBlobRepositoryOptions"));
            services.Configure<DatabaseOptions>(configuration.GetSection("InvoiceTemplateDatabaseOptions"));
        }

        protected override ICompositionModule[] GetCompositionModules()
        {
            return new ICompositionModule[] 
            {
                new Business.CompositionModule.CompositionModule(),
                new DataAccess.CompositionModule.CompositionModule()
            };
        }

        protected override string serviceName => "PWP.InvoiceCapture.OCR.DataAnalysis.API";

        protected override string apiVersion => "v1";
    }
}
