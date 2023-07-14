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
    internal class EmailDocumentUploadedMessageHandler : MessageHandlerBase<EmailDocumentUploadedMessage>
    {
        public EmailDocumentUploadedMessageHandler(ITenantService tenantService, ICultureService cultureService, ITenantSettingService tenantSettingService, IServiceBusPublisher publisher, ITelemetryClient telemetryClient, IApplicationContext applicationContext)
            : base(telemetryClient, applicationContext)
        {
            Guard.IsNotNull(tenantService, nameof(tenantService));
            Guard.IsNotNull(publisher, nameof(publisher));
            Guard.IsNotNull(cultureService, nameof(cultureService));
            Guard.IsNotNull(tenantSettingService, nameof(tenantSettingService));

            this.tenantService = tenantService;
            this.publisher = publisher;
            this.cultureService = cultureService;
            this.tenantSettingService = tenantSettingService;
        }

        protected override async Task HandleMessageAsync(EmailDocumentUploadedMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotNullOrWhiteSpace(message.To, nameof(message.To));

            var tenantId = await tenantService.GetTenantIdByEmailAsync(message.To, cancellationToken);
            
            if (tenantId != null)
            {
                //TODO: Logic is duplicated in ResourseOwnerPasswordValidator
                var tenantSetting = await tenantSettingService.GetAsync(GetTenantId(tenantId), CancellationToken.None);
                var cultureName = "en-US";

                if (tenantSetting != null)
                {
                    var culture = await cultureService.GetAsync(tenantSetting.CultureId, CancellationToken.None);
                    if (culture != null)
                    {
                        cultureName = culture.Name;
                    }
                }
                await PublishMessageAsync(message, tenantId, cultureName, cancellationToken);
            }
        }

        private async Task PublishMessageAsync(EmailDocumentUploadedMessage message, string tenantId, string cultureName, CancellationToken cancellationToken)
        {
            var tenantEmailResolvedMessage = new TenantEmailResolvedMessage
            {
                To = message.To,
                From = message.From,
                FileId = message.FileId,
                FileName = message.FileName,
                FileSourceType = message.FileSourceType,
                TenantId = tenantId,
                CultureName = cultureName
            };

            await publisher.PublishAsync(tenantEmailResolvedMessage, cancellationToken);
        }

        //TODO: move to core
        private int GetTenantId(string tenantId)
        {
            if (string.Equals(tenantId, "Default"))
            {
                return 1;
            }

            return Convert.ToInt32(tenantId);
        }

        private readonly ITenantService tenantService;
        private readonly IServiceBusPublisher publisher;
        private readonly ICultureService cultureService;
        private readonly ITenantSettingService tenantSettingService;
    }
}
