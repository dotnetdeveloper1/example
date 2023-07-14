using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Service.MessageHandlers
{
    internal class InvoiceCreatedMessageHandler : MessageHandlerBase<InvoiceCreatedMessage>
    {
        public InvoiceCreatedMessageHandler(IUsageService usageCalculationService, IServiceBusPublisher publisher, ITelemetryClient telemetryClient, IApplicationContext applicationContext)
            : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(usageCalculationService, nameof(usageCalculationService));
            Guard.IsNotNull(publisher, nameof(publisher));

            this.usageCalculationService = usageCalculationService;
            this.publisher = publisher;
        }

        protected override async Task HandleMessageAsync(InvoiceCreatedMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotZeroOrNegative(message.InvoiceId, nameof(message.InvoiceId));
            Guard.IsNotNullOrWhiteSpace(message.FileId, nameof(message.FileId));
            Guard.IsNotNullOrWhiteSpace(message.CultureName, nameof(message.CultureName));

            var limitNotExceeded = 
                await usageCalculationService.TryIncreaseCountOfUploadedInvoicesAsync(GetTenantId(), cancellationToken);

            if (limitNotExceeded)
            {
                await PublishLimitNotExceededMessageAsync(message, cancellationToken);
            }
            else
            {
                await PublishLimitExceededMessageAsync(message, cancellationToken);
            }
        }

        //TODO: move to core
        private int GetTenantId()
        {
            var tenantId = applicationContext.TenantId;
            if (string.Equals(tenantId, "Default"))
            {
                return 1;
            }

            return Convert.ToInt32(tenantId);
        }

        private async Task PublishLimitNotExceededMessageAsync(InvoiceCreatedMessage message, CancellationToken cancellationToken)
        {
            var invoiceProcessingLimitNotExceededMessage = new InvoiceProcessingLimitNotExceededMessage
            {
                FileId = message.FileId,
                InvoiceId = message.InvoiceId,
                TenantId = message.TenantId,
                CultureName = message.CultureName
            };

            await publisher.PublishAsync(invoiceProcessingLimitNotExceededMessage, cancellationToken);
        }

        private async Task PublishLimitExceededMessageAsync(InvoiceCreatedMessage message, CancellationToken cancellationToken)
        {
            var invoiceProcessingLimitExceededMessage = new InvoiceProcessingLimitExceededMessage
            {
                FileId = message.FileId,
                InvoiceId = message.InvoiceId,
                TenantId = message.TenantId
            };

            await publisher.PublishAsync(invoiceProcessingLimitExceededMessage, cancellationToken);
        }

        private readonly IUsageService usageCalculationService;
        private readonly IServiceBusPublisher publisher;
    }
}
