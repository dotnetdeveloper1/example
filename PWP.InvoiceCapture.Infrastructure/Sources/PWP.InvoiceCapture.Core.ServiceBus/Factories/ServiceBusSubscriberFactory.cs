using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Management;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Services;
using PWP.InvoiceCapture.Core.Utilities;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.Core.ServiceBus.Factories
{
    public class ServiceBusSubscriberFactory : IServiceBusSubscriberFactory
    {
        public ServiceBusSubscriberFactory(IManagementClientFactory managementClientFactory, ISubscriptionClientFactory subscriptionClientFactory, IMessageTypeProvider messageTypeProvider, IMessageSerializer messageSerializer)
        {
            Guard.IsNotNull(managementClientFactory, nameof(managementClientFactory));
            Guard.IsNotNull(subscriptionClientFactory, nameof(subscriptionClientFactory));
            Guard.IsNotNull(messageTypeProvider, nameof(messageTypeProvider));
            Guard.IsNotNull(messageSerializer, nameof(messageSerializer));

            this.managementClientFactory = managementClientFactory;
            this.subscriptionClientFactory = subscriptionClientFactory;
            this.messageTypeProvider = messageTypeProvider;
            this.messageSerializer = messageSerializer;
        }

        public ServiceBusSubscriberFactory()
        {
            managementClientFactory = new DefaultManagementClientFactory();
            subscriptionClientFactory = new DefaultSubscriptionClientFactory();
            messageTypeProvider = new DefaultMessageTypeProvider();
            messageSerializer = new DefaultMessageSerializer(new SerializationService());
        }

        public IServiceBusSubscriber Create(IEnumerable<IMessageHandler> messageHandlers, IExceptionHandler exceptionHandler, ServiceBusSubscriberOptions options)
        {
            Guard.IsNotNullOrEmpty(messageHandlers, nameof(messageHandlers));
            Guard.IsNotNull(exceptionHandler, nameof(exceptionHandler));
            Guard.IsNotNull(options, nameof(options));

            return new ServiceBusSubscriber(managementClientFactory, subscriptionClientFactory, messageTypeProvider, exceptionHandler, messageHandlers, messageSerializer, options);
        }

        private readonly IManagementClientFactory managementClientFactory;
        private readonly ISubscriptionClientFactory subscriptionClientFactory;
        private readonly IMessageTypeProvider messageTypeProvider;
        private readonly IMessageSerializer messageSerializer;
    }
}
