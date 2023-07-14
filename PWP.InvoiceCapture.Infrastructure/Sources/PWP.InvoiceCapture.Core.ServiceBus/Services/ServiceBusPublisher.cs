using Microsoft.Azure.ServiceBus;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Services
{
    public class ServiceBusPublisher : IServiceBusPublisher
    {
        public ServiceBusPublisher(IManagementClientFactory managementClientFactory, ITopicClientFactory topicClientFactory, IMessageIdentityProvider messageIdentityProvider, IMessageTypeProvider messageTypeProvider, IMessageSerializer messageSerializer, ServiceBusPublisherOptions options) 
        {
            Guard.IsNotNull(managementClientFactory, nameof(managementClientFactory));
            Guard.IsNotNull(topicClientFactory, nameof(topicClientFactory));
            Guard.IsNotNull(messageIdentityProvider, nameof(messageIdentityProvider));
            Guard.IsNotNull(messageTypeProvider, nameof(messageTypeProvider));
            Guard.IsNotNull(messageSerializer, nameof(messageSerializer));
            ValidatePublisherOptions(options);

            this.managementClientFactory = managementClientFactory;
            this.topicClientFactory = topicClientFactory;
            this.messageIdentityProvider = messageIdentityProvider;
            this.messageTypeProvider = messageTypeProvider;
            this.messageSerializer = messageSerializer;
            this.options = options;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var managementClient = await managementClientFactory.CreateAsync(options.ConnectionString, cancellationToken);

            if (managementClient == null)
            {
                throw new InvalidOperationException("Cannot start ServiceBusPublisher because ManagementClientFactory returned null.");
            }

            await managementClient.CreateOrUpdateTopicAsync(options.TopicName, cancellationToken);

            topicClient = topicClientFactory.Create(options.ConnectionString, options.TopicName, retryPolicy);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (topicClient != null)
            {
                await topicClient.CloseAsync();
            }
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
        {
            await PublishMessageAsync(message, null, cancellationToken);
        }

        public async Task PublishAsync<TMessage>(TMessage message, string correlationId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(correlationId, nameof(correlationId));

            await PublishMessageAsync(message, correlationId, cancellationToken);
        }

        private async Task PublishMessageAsync<TMessage>(TMessage message, string correlationId, CancellationToken cancellationToken) 
        {
            Guard.IsNotNull(message, nameof(message));

            if (topicClient == null)
            {
                throw new InvalidOperationException("Cannot publish message because ServiceBusPublisher was not started.");
            }

            var serviceBusMessage = new Message
            {
                MessageId = await messageIdentityProvider.GetNextAsync(cancellationToken),
                CorrelationId = correlationId,
                Label = messageTypeProvider.Get(message.GetType()),
                Body = messageSerializer.Serialize(message),
                TimeToLive = messageTimeToLive
            };

            await topicClient.SendAsync(serviceBusMessage);
        }

        private void ValidatePublisherOptions(ServiceBusPublisherOptions options) 
        {
            Guard.IsNotNull(options, nameof(options));
            Guard.IsNotNullOrWhiteSpace(options.ConnectionString, nameof(options.ConnectionString));
            Guard.IsNotNullOrWhiteSpace(options.TopicName, nameof(options.TopicName));
        }

        private ITopicClient topicClient;
        private readonly RetryPolicy retryPolicy = RetryPolicy.Default;
        private readonly TimeSpan messageTimeToLive = TimeSpan.MaxValue;
        private readonly IManagementClientFactory managementClientFactory;
        private readonly ITopicClientFactory topicClientFactory;
        private readonly IMessageIdentityProvider messageIdentityProvider;
        private readonly IMessageTypeProvider messageTypeProvider;
        private readonly IMessageSerializer messageSerializer;
        private readonly ServiceBusPublisherOptions options;
    }
}
