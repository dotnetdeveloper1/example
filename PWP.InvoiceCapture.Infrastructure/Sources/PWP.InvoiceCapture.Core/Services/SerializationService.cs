using Newtonsoft.Json;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Runtime.Serialization;

namespace PWP.InvoiceCapture.Core.Services
{
    public class SerializationService : ISerializationService
    {
        public string Serialize<T>(T serializableObject) where T : class
        {
            Guard.IsNotNull(serializableObject, nameof(serializableObject));
            
            try
            {
                return JsonConvert.SerializeObject(serializableObject);
            }
            catch (JsonSerializationException exception)
            {
                throw new SerializationException(exception.Message, exception);
            }
        }

        public T Deserialize<T>(string json) where T : class
        {
            Guard.IsNotNullOrWhiteSpace(json, nameof(json));

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonSerializationException exception)
            {
                throw new SerializationException(exception.Message, exception);
            }
        }

        public object Deserialize(string json, Type type)
        {
            Guard.IsNotNullOrWhiteSpace(json, nameof(json));
            Guard.IsNotNull(type, nameof(type));

            try
            {
                return JsonConvert.DeserializeObject(json, type);
            }
            catch (JsonSerializationException exception)
            {
                throw new SerializationException(exception.Message, exception);
            }
        }
    }
}
