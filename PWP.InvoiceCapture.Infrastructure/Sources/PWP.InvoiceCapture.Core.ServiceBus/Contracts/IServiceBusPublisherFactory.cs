using PWP.InvoiceCapture.Core.ServiceBus.Models;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface IServiceBusPublisherFactory
    {
        IServiceBusPublisher Create(ServiceBusPublisherOptions options);
    }
}
