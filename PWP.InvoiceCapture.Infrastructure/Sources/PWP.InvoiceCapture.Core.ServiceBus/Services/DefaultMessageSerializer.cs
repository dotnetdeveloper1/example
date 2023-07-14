using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Text;

namespace PWP.InvoiceCapture.Core.ServiceBus.Services
{
    public class DefaultMessageSerializer : IMessageSerializer
    {
        public DefaultMessageSerializer(ISerializationService serializationService)
        {
            Guard.IsNotNull(serializationService, nameof(serializationService));
            this.serializationService = serializationService;
        }

        public object Deserialize(byte[] buffer, Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            if (buffer == null || buffer.Length == 0)
            {
                return null;
            }

            var json = Encoding.UTF8.GetString(buffer);

            return serializationService.Deserialize(json, type);
        }

        public byte[] Serialize(object message)
        {
            if (message == null)
            {
                return null;
            }
            
            var json = serializationService.Serialize(message);

            return Encoding.UTF8.GetBytes(json);
        }

        private readonly ISerializationService serializationService;
    }
}
