using Microsoft.Azure.ServiceBus;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface ISubscriptionClientFactory
    {
        ISubscriptionClient Create(string connectionString, string topicName, string subscriptionName, ReceiveMode receiveMode, RetryPolicy retryPolicy);
    }
}
