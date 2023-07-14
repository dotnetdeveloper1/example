using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Management;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Services;
using PWP.InvoiceCapture.Core.Utilities;

namespace PWP.InvoiceCapture.Core.ServiceBus.Factories
{
    public class ServiceBusPublisherFactory : IServiceBusPublisherFactory
    {
        public ServiceBusPublisherFactory(IManagementClientFactory managementClientFactory, ITopicClientFactory topicClientFactory, IMessageIdentityProvider messageIdentityProvider, IMessageTypeProvider messageTypeProvider, IMessageSerializer messageSerializer) 
        {
            Guard.IsNotNull(managementClientFactory, nameof(managementClientFactory));
            Guard.IsNotNull(topicClientFactory, nameof(topicClientFactory));
            Guard.IsNotNull(messageIdentityProvider, nameof(messageIdentityProvider));
            Guard.IsNotNull(messageTypeProvider, nameof(messageTypeProvider));
            Guard.IsNotNull(messageSerializer, nameof(messageSerializer));

            this.managementClientFactory = managementClientFactory;
            this.topicClientFactory = topicClientFactory;
            this.messageIdentityProvider = messageIdentityProvider;
            this.messageTypeProvider = messageTypeProvider;
            this.messageSerializer = messageSerializer;
        }

        public ServiceBusPublisherFactory() 
        {
            managementClientFactory = new DefaultManagementClientFactory();
            topicClientFactory = new DefaultTopicClientFactory();
            messageIdentityProvider = new DefaultMessageIdentityProvider();
            messageTypeProvider = new DefaultMessageTypeProvider();
            messageSerializer = new DefaultMessageSerializer(new SerializationService());
        }

        public IServiceBusPublisher Create(ServiceBusPublisherOptions options)
        {
            Guard.IsNotNull(options, nameof(options));

            return new ServiceBusPublisher(managementClientFactory, topicClientFactory, messageIdentityProvider, messageTypeProvider, messageSerializer, options);
        }

        private readonly IManagementClientFactory managementClientFactory;
        private readonly ITopicClientFactory topicClientFactory;
        private readonly IMessageIdentityProvider messageIdentityProvider;
        private readonly IMessageTypeProvider messageTypeProvider;
        private readonly IMessageSerializer messageSerializer;
    }
}
