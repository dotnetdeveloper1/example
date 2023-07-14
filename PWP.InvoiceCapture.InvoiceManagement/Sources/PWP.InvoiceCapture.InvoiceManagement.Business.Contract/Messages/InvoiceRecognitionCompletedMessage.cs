using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages
{
    public class InvoiceRecognitionCompletedMessage : ServiceBusMessageBase
    {
        public int InvoiceId { get; set; }
        public string TemplateId { get; set; }
        public string DataAnnotationFileId { get; set; }
        public string CultureName { get; set; }
        public int TrainingFileCount { get; set; }
        public InvoiceProcessingType InvoiceProcessingType { get; set; }
    }
}
