using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages
{
    public class InvoiceDocumentUploadedMessage : ServiceBusMessageBase
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FromEmailAddress { get; set; }
        public string CultureName { get; set; }
        public FileSourceType FileSourceType { get; set; }
    }
}
