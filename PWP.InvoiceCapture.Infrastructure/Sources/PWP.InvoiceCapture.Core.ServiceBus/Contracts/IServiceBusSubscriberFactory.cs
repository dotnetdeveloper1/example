using PWP.InvoiceCapture.Core.ServiceBus.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface IServiceBusSubscriberFactory
    {
        IServiceBusSubscriber Create(IEnumerable<IMessageHandler> messageHandlers, IExceptionHandler exceptionHandler, ServiceBusSubscriberOptions options);
    }
}
