using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Notification;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Notification
{
    internal class NotificationClientFactory : INotificationClientFactory
    {
        public INotificationClient Create(string baseUrl)
        {
            return new NotificationClient(
                new Core.Models.ApiClientOptions()
                {
                    RetryAttemptCount = 3,
                    BaseAddress = baseUrl,
                    TimeoutInSeconds = 1200
                });
        }
    }
}
