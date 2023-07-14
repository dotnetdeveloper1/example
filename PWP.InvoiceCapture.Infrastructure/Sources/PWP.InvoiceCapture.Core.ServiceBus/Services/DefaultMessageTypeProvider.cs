using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using System;

namespace PWP.InvoiceCapture.Core.ServiceBus.Services
{
    public class DefaultMessageTypeProvider : IMessageTypeProvider
    {
        public string Get(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return type.Name.ToLower(); 
        }
    }
}
