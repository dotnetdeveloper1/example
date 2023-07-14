using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class WebhookNotification
    {
        public TriggerType TriggerType { get; set; }
        public string InvoiceUrl { get; set; }
    }
}
