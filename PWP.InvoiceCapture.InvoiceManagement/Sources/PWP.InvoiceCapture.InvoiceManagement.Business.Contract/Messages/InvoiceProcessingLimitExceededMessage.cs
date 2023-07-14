using PWP.InvoiceCapture.Core.ServiceBus.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages
{
    public class InvoiceProcessingLimitExceededMessage : ServiceBusMessageBase
    {
        public int InvoiceId { get; set; }
        public string FileId { get; set; }
    }
}
