using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Notification;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class NotificationService : INotificationService
    {
        public NotificationService(IWebhookService webhookService, INotificationClientFactory notificationClientFactory, IOptions<NotificationOptions> optionsAccessor) 
        {
            Guard.IsNotNull(webhookService, nameof(webhookService));
            Guard.IsNotNull(notificationClientFactory, nameof(notificationClientFactory));
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            GuardOptions(optionsAccessor.Value);

            this.webhookService = webhookService;
            this.notificationClientFactory = notificationClientFactory;
            this.optionsAccessor = optionsAccessor;
        }

        public async Task NotifyAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            var webhooks = await webhookService.GetListAsync(cancellationToken);
            
            if (webhooks == null)
            {
                return;
            }

            var statusChangedHook = webhooks.FirstOrDefault(webhook => webhook.TriggerType == TriggerType.StatusChanged);

            if (statusChangedHook != null)
            {
                var notificationOptions = optionsAccessor.Value;
                var urlPattern = notificationOptions.InvoiceUrlPattern;
                var invoiceUrl = urlPattern.Replace("{id}", invoiceId.ToString());
                var notification = new WebhookNotification() { InvoiceUrl = invoiceUrl, TriggerType = TriggerType.StatusChanged };
                var notificationClient = notificationClientFactory.Create(statusChangedHook.Url);
                await notificationClient.NotifyAsync(notification, cancellationToken);
            }
        }

        private void GuardOptions(NotificationOptions notificationOptions)
        {
            Guard.IsNotNull(notificationOptions, nameof(notificationOptions));
            Guard.IsNotNullOrWhiteSpace(notificationOptions.InvoiceUrlPattern, nameof(notificationOptions.InvoiceUrlPattern));
        }

        private readonly INotificationClientFactory notificationClientFactory;
        private readonly IWebhookService webhookService;
        private readonly IOptions<NotificationOptions> optionsAccessor;
    }
}
