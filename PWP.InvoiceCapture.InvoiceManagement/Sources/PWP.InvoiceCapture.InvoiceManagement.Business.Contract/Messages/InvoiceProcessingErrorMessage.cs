using PWP.InvoiceCapture.Core.ServiceBus.Models;
using System;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages
{
    public class InvoiceProcessingErrorMessage : ServiceBusMessageBase
    {
        public int InvoiceId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
