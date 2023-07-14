using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages
{
    public class EmailDocumentUploadedMessage : ServiceBusMessageBase
    {
        public string To { get; set; }
        public string From { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public FileSourceType FileSourceType { get; set; }
    }
}
