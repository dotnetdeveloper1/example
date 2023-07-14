using System;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface IMessageTypeProvider
    {
        string Get(Type type);
    }
}
