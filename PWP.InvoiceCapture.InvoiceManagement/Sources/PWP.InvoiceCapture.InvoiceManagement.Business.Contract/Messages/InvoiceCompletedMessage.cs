using PWP.InvoiceCapture.Core.ServiceBus.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages
{
    public class InvoiceCompletedMessage : ServiceBusMessageBase
    {
        public int InvoiceId { get; set; }
        public string TemplateId { get; set; }
        public string InvoiceFileId { get; set; }
        public string DataAnnotationFileId { get; set; }
    }
}
