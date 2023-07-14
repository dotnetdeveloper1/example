using PWP.InvoiceCapture.Core.ServiceBus.Models;

namespace PWP.InvoiceCapture.Core.ServiceBus.Samples
{
    public class Message : ServiceBusMessageBase
    {
        public int Property { get; set; }
    }
}
