using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories;
using PWP.InvoiceCapture.OCR.DataAnalysis.DataAccess.Database;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.DataAccess.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer();
            services.AddTransient<ITrainingBlobRepository, TrainingBlobRepository>();
            services.AddTransient<IDatabaseContextFactory, OCRDatabaseContextFactory>();
            services.AddTransient<IInvoiceTemplateRepository, InvoiceTemplateRepository>();
            services.AddTransient<IFormRecognizerResourceRepository, FormRecognizerResourceRepository>();
        }
    }
}
