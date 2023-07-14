using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.Services;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Services;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Factories;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Services;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.Business.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddTransient<IFormRecognizerTrainingDocumentFactory, FormRecognizerTrainingDocumentFactory>();
            services.AddTransient<ITemplateManagementService, TemplateManagementService>();
            services.AddTransient<IFileNameProvider, FileNameProvider>();
        }
    }
}
