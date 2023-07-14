using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.OCR.Core.Contract.Services;
using PWP.InvoiceCapture.OCR.Core.Contracts;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFormRecognizerClient(this IServiceCollection services)
        {
            services.AddTransient<IFormRecognizerClient, FormRecognizerClientPoolDecorator>();
            services.AddTransient<IFormRecognizerClientService, FormRecognizerClientService>();
        }
    }
}
