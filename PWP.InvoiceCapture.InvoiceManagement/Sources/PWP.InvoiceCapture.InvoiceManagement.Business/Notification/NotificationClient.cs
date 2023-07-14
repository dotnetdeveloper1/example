using Newtonsoft.Json;
using PWP.InvoiceCapture.Core.Communication;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Notification;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Notification
{
    internal class NotificationClient : ApiClientBase, INotificationClient
    {
        public NotificationClient(ApiClientOptions options) : base(options)
        {
        }

        public async Task NotifyAsync(WebhookNotification notification, CancellationToken cancelationToken)
        {
            var json = JsonConvert.SerializeObject(notification);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            await ExecuteWithRetryAsync(() =>
                client.PostAsync("", httpContent, cancelationToken));
        }
    }
}
