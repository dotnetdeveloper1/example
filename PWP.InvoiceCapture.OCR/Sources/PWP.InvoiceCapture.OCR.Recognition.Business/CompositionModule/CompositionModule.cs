using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.Services;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using PWP.InvoiceCapture.OCR.Recognition.Business.Factories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Services;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddTransient<IRecognitionEngine, RecognitionEngine>();
            services.AddTransient<IRecognitionElementFactory, RecognitionElementFactory>();
            services.AddTransient<IOCRElementsFactory, OCRElementsFactory>();
            services.AddTransient<IDataAnnotationsFactory, DataAnnotationsFactory>();
            services.AddTransient<IInvoiceAnalysisService, InvoiceAnalysisService>();
            services.AddTransient<ILabelExtractorService, LabelExtractorService>();
            services.AddTransient<ILabelOfInterestService, LabelOfInterestService>();
            services.AddTransient<ILineService, LineService>();
            services.AddTransient<IRegionService, RegionService>();
            services.AddTransient<IWordService, WordService>();
            services.AddTransient<IGeometricFeaturesService, GeometricFeaturesService>();
            services.AddTransient<IOCRProviderRecognitionDataFactory, FormRecognizerRecognitionDataFactory>();
            services.AddTransient<IFormRecognizerKeyConversionService, FormRecognizerKeyConversionService>();
            services.AddTransient<IPostOcrTemplateMatchingService, PostOcrTemplateMatchingService>();
            services.AddTransient<IFileNameProvider, FileNameProvider>();
            services.AddTransient<ITenantTrackingService, TenantTrackingService>();
        }
    }
}
