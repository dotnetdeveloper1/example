using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Contracts;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Database;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Repositories;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer();
            services.AddTransient<ITrainingBlobRepository, TrainingBlobRepository>();
            services.AddTransient<IRecognitionDatabaseContextFactory, RecognitionDatabaseContextFactory>();
            services.AddSingleton<ILabelOfInterestRepository, LabelOfInterestRepository>();
            services.AddTransient<IDatabaseContextFactory, OCRDatabaseContextFactory>();
            services.AddTransient<IInvoiceTemplateRepository, InvoiceTemplateRepository>();
            services.AddTransient<IFormRecognizerResourceRepository, FormRecognizerResourceRepository>();
            services.AddTransient<ITenantRepository, TenantRepository>();
        }
    }
}
