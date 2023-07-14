using PWP.InvoiceCapture.Core.ServiceBus.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface IMessageHandler
    {
        Type MessageType { get; }

        Task HandleAsync(BrokeredMessage brokeredMessage, CancellationToken cancellationToken);
    }
}
