namespace PWP.InvoiceCapture.Core.ServiceBus.Models
{
    public class ServiceBusPublisherOptions
    {
        public string ConnectionString { get; set; }
        public string TopicName { get; set; }
    }
}
