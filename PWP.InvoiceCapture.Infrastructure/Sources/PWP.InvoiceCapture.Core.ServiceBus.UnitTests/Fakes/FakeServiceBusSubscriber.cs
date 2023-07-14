using Microsoft.Azure.ServiceBus;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Fakes
{
    [ExcludeFromCodeCoverage]
    internal class FakeServiceBusSubscriber : ServiceBusSubscriber
    {
        public FakeServiceBusSubscriber(IManagementClientFactory managementClientFactory, ISubscriptionClientFactory subscriptionClientFactory, IMessageTypeProvider messageTypeProvider, IExceptionHandler exceptionHandler, IEnumerable<IMessageHandler> messageHandlers, IMessageSerializer messageSerializer, ServiceBusSubscriberOptions options) 
            : base(managementClientFactory, subscriptionClientFactory, messageTypeProvider, exceptionHandler, messageHandlers, messageSerializer, options)
        { }

        public new Task ProcessMessagesAsync(Message message, CancellationToken cancellationToken) => 
            base.ProcessMessagesAsync(message, cancellationToken);
    }
}
