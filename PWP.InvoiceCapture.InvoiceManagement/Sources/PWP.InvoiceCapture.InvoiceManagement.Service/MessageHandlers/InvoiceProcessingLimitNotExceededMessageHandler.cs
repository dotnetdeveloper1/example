using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service.MessageHandlers
{
    internal class InvoiceProcessingLimitNotExceededMessageHandler : MessageHandlerBase<InvoiceProcessingLimitNotExceededMessage>
    {
        public InvoiceProcessingLimitNotExceededMessageHandler(IFieldService fieldService, IInvoiceDocumentService invoiceDocumentService, IInvoicePageService invoicePageService, IServiceBusPublisher publisher, ITelemetryClient telemetryClient, IApplicationContext applicationContext) 
            : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(fieldService, nameof(fieldService));
            Guard.IsNotNull(invoiceDocumentService, nameof(invoiceDocumentService));
            Guard.IsNotNull(invoicePageService, nameof(invoicePageService));
            Guard.IsNotNull(publisher, nameof(publisher));

            this.fieldService = fieldService;
            this.invoiceDocumentService = invoiceDocumentService;
            this.invoicePageService = invoicePageService;
            this.publisher = publisher;
        }

        protected override async Task HandleMessageAsync(InvoiceProcessingLimitNotExceededMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotNullOrWhiteSpace(message.FileId, nameof(message.FileId));
            Guard.IsNotZeroOrNegative(message.InvoiceId, nameof(message.InvoiceId));
            Guard.IsNotNullOrWhiteSpace(message.CultureName, nameof(message.CultureName));

            var fieldsTask = fieldService.GetListAsync(cancellationToken);

            var pages = await invoiceDocumentService.GetInvoicePagesAsync(message.InvoiceId, message.FileId, cancellationToken);
            await invoicePageService.CreateAsync(pages, cancellationToken);

            var fields = await fieldsTask;

            await PublishMessageAsync(fields, pages, message.InvoiceId, message.FileId, message.CultureName, cancellationToken);
        }

        private async Task PublishMessageAsync(List<Field> fields, List<InvoicePage> invoicePages, int invoiceId, string fileId, string cultureName, CancellationToken cancellationToken)
        {
            var invoicePagesCreatedMessage = new InvoiceReadyForRecognitionMessage()
            {
                InvoiceId = invoiceId,
                Pages = invoicePages,
                FileId = fileId,
                TenantId = applicationContext.TenantId,
                CultureName = cultureName,
                Fields = fields
            };

            await publisher.PublishAsync(invoicePagesCreatedMessage, cancellationToken);
        }

        private readonly IFieldService fieldService;
        private readonly IInvoiceDocumentService invoiceDocumentService;
        private readonly IInvoicePageService invoicePageService;
        private readonly IServiceBusPublisher publisher;
    }
}
