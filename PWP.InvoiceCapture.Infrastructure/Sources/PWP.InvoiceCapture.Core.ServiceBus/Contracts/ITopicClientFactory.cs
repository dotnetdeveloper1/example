using Microsoft.Azure.ServiceBus;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface ITopicClientFactory
    {
        ITopicClient Create(string connectionString, string topicName, RetryPolicy retryPolicy);
    }
}
