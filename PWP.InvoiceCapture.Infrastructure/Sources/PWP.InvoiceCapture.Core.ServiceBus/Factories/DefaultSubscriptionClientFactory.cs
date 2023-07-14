using Microsoft.Azure.ServiceBus;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.Utilities;

namespace PWP.InvoiceCapture.Core.ServiceBus.Factories
{
    public class DefaultSubscriptionClientFactory : ISubscriptionClientFactory
    {
        public ISubscriptionClient Create(string connectionString, string topicName, string subscriptionName, ReceiveMode receiveMode, RetryPolicy retryPolicy)
        {
            Guard.IsNotNullOrWhiteSpace(connectionString, nameof(connectionString));
            Guard.IsNotNullOrWhiteSpace(topicName, nameof(topicName));
            Guard.IsNotNullOrWhiteSpace(subscriptionName, nameof(subscriptionName));
            Guard.IsNotNull(retryPolicy, nameof(retryPolicy));

            return new SubscriptionClient(connectionString, topicName, subscriptionName, receiveMode, retryPolicy);
        }
    }
}
