using Microsoft.Azure.ServiceBus;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.Utilities;

namespace PWP.InvoiceCapture.Core.ServiceBus.Factories
{
    public class DefaultTopicClientFactory : ITopicClientFactory
    {
        public ITopicClient Create(string connectionString, string topicName, RetryPolicy retryPolicy)
        {
            Guard.IsNotNullOrWhiteSpace(connectionString, nameof(connectionString));
            Guard.IsNotNullOrWhiteSpace(topicName, nameof(topicName));
            Guard.IsNotNull(retryPolicy, nameof(retryPolicy));

            return new TopicClient(connectionString, topicName, retryPolicy);
        }
    }
}
