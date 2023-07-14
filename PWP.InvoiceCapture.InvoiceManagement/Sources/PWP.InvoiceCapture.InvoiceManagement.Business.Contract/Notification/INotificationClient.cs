using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Notification
{
    public interface INotificationClient
    {
        Task NotifyAsync(WebhookNotification notification, CancellationToken cancelationToken);
    }
}
