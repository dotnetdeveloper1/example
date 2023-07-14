using System;

namespace PWP.InvoiceCapture.Core.ServiceBus.Contracts
{
    public interface IMessageSerializer
    {
        byte[] Serialize(object message);
        object Deserialize(byte[] buffer, Type type);
    }
}
