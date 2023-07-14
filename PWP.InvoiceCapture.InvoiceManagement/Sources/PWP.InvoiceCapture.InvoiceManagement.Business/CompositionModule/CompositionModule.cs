using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Core.CompositionModule;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Notification;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Mappers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Notification;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.CompositionModule
{
    public class CompositionModule : ICompositionModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            // Services
            services.AddTransient<IInvoicePageService, InvoicePageService>();
            services.AddTransient<IInvoiceService, InvoiceService>();
            services.AddTransient<IInvoiceProcessingResultService, InvoiceProcessingResultService>();
            services.AddTransient<IPdfService, PdfService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<ICalculationService, CalculationService>();
            services.AddTransient<IInvoiceDocumentService, InvoiceDocumentService>();
            services.AddTransient<IInvoiceLineService, InvoiceLineService>();
            services.AddTransient<IFieldService, FieldService>();
            services.AddTransient<IFieldGroupService, FieldGroupService>();
            services.AddTransient<IInvoiceFieldService, InvoiceFieldService>();
            services.AddTransient<IFormulaExtractionService, FormulaExtractionService>();
            services.AddTransient<IWebhookService, WebhookService>();
            services.AddTransient<INotificationService, NotificationService>();

            // Validators
            services.AddSingleton<IRequiredValidator, RequiredValidator>();
            services.AddSingleton<IRequiredValueValidator, RequiredValueValidator>();
            services.AddSingleton<IDateValidator, DateValidator>();
            services.AddSingleton<IMaxLengthValidator, MaxLengthValidator>();
            services.AddSingleton<IMinLengthValidator, MinLengthValidator>();
            services.AddSingleton<IMaxValueValidator, MaxValueValidator>();
            services.AddSingleton<IMinValueValidator, MinValueValidator>();
            services.AddSingleton<IDecimalValidator, DecimalValidator>();
            services.AddSingleton<ISingleFieldPerTypeValidator, SingleFieldPerTypeValidator>();
            services.AddSingleton<ILineOrderValidator, LineOrderValidator>();
            services.AddSingleton<ITotalMultiplicationValidator, TotalMultiplicationValidator>();
            services.AddSingleton<IDataAnnotationValidator, DataAnnotationValidator>();
            services.AddSingleton<IHourValidator, HourValidator>();
            services.AddSingleton<IQuantityValidator, QuantityValidator>();
            services.AddSingleton<IDataAnnotationValidator, DataAnnotationValidator>();
            services.AddSingleton<IFormulaValidator, FormulaValidator>();
            services.AddSingleton<IFormulaExecutionResultValidator, FormulaExecutionResultValidator>();

            // Mappers
            services.AddSingleton<IAnnotationMapper, AnnotationMapper>();

            services.AddSingleton<INotificationClientFactory, NotificationClientFactory>();
        }
    }
}
