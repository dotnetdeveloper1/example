using System;

namespace PWP.InvoiceCapture.Core.ServiceBus.Models
{
    public class ServiceBusSubscriberOptions
    {
        public string ConnectionString { get; set; }
        public string TopicName { get; set; }
        public string SubscriberName { get; set; }
        public TimeSpan MessageLockDuration { get; set; } = TimeSpan.FromSeconds(30);
        public int MaxConcurrentCalls { get; set; } = 50;
    }
}
