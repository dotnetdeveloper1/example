using PWP.InvoiceCapture.Core.ServiceBus.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages
{
    public class InvoiceProcessingLimitNotExceededMessage : ServiceBusMessageBase
    {
        public int InvoiceId { get; set; }
        public string FileId { get; set; }
        public string CultureName { get; set; }
    }
}
