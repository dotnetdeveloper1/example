using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Core.DataAccess.Services;
using PWP.InvoiceCapture.Core.Extensions;
using PWP.InvoiceCapture.Core.Services;
using PWP.InvoiceCapture.Document.API.Client.Extensions;
using PWP.InvoiceCapture.Document.API.Client.Options;
using PWP.InvoiceCapture.OCR.Core;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Extensions;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Options;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Mappers;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Options;
using PWP.InvoiceCapture.OCR.Recognition.Business.Mapper;
using System;
using System.IO;
using System.Windows.Forms;
using TroubleShootingApp.Contracts;
using TroubleShootingApp.DataAccess;
using TroubleShootingApp.Options;
using TroubleShootingApp.Services;

namespace TroubleShootingApp
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
                                 .AddEnvironmentVariables();

            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddFormRecognizerClient();
            services.AddDocumentApiClient();
            services.AddSerializationService();
            services.AddInvoiceTemplates();
            services.AddApplicationInsightsTelemetryWorkerService();
            services.AddTelemetryClient();
            foreach (var module in GetCompositionModules())
            {
                module.RegisterTypes(services);
            }
            services.Configure<FormRecognizerClientPoolOptions>(configuration.GetSection("FormRecognizerClientPoolOptions"));
            services.Configure<DocumentApiClientOptions>(configuration.GetSection("DocumentApiClientOptions"));
            services.Configure<RecognitionDatabaseOptions>(configuration.GetSection("InvoiceTemplateDatabaseOptions"));
            services.Configure<TenantDbOptions>(configuration.GetSection("TenantsDatabaseOptions"));
            services.Configure<InvoiceDbOptions>(configuration.GetSection("InvoicesDatabaseOptions"));
            services.Configure< PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options.DatabaseOptions>(configuration.GetSection("InvoiceTemplateDatabaseOptions"));
            services.Configure<DocumentStorageOptions>(configuration.GetSection("TrainingBlobRepositoryOptions"));
            services.AddTransient<IApplicationRunner,ApplicationRunner>();
            services.AddTransient<ITenantRepository, TenantsRepository>();
            services.AddTransient<IInvoicesDatabaseNameProvider, InvoicesDatabaseNameProvider>();
            services.AddTransient<IInvoicesRepository, InvoiceRepository>();
            services.AddTransient<IFieldsRepository, FieldsRepository>();
            services.AddTransient<IFieldMapper, FieldMapper>();
            var provider = services.BuildServiceProvider();
            var runner = provider.GetService<IApplicationRunner>();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InvoiceName(runner));
        }

        private static ICompositionModule[] GetCompositionModules()
        {
            return new ICompositionModule[]
            {
                new PWP.InvoiceCapture.OCR.Recognition.DataAccess.CompositionModule.CompositionModule(),
                new PWP.InvoiceCapture.OCR.Recognition.Business.CompositionModule.CompositionModule(),
                new PWP.InvoiceCapture.OCR.DataAnalysis.DataAccess.CompositionModule.CompositionModule()
        };
        }
    }
}
