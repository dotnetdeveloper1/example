namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Notification
{
    public interface INotificationClientFactory
    {
         INotificationClient Create(string baseUrl);
    }
}
