namespace PWP.InvoiceCapture.Core.ServiceBus.Models
{
    public class BrokeredMessage
    {
        public string Id { get; set; }
        public string CorrelationId { get; set; }
        public string MessageType { get; set; }
        public object InnerMessage { get; set; }
    }
}
