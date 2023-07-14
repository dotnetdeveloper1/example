using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface IServiceBusPublisher
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken);
        Task PublishAsync<TMessage>(TMessage message, string correlationId, CancellationToken cancellationToken);
    }
}
