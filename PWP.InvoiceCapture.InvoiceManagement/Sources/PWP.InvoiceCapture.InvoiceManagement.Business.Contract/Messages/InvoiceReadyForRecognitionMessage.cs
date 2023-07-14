using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages
{
    public class InvoiceReadyForRecognitionMessage : ServiceBusMessageBase
    {
        public int InvoiceId { get; set; }
        public string FileId { get; set; }
        public string CultureName { get; set; }
        public List<InvoicePage> Pages { get; set; }
        public List<Field> Fields { get; set; }
    }
}
