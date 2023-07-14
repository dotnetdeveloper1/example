using System;

namespace PWP.InvoiceCapture.Core.Contracts
{
    public interface ISerializationService
    {
        string Serialize<T>(T serializableObject) where T : class;
        T Deserialize<T>(string json) where T : class;
        object Deserialize(string json, Type type);
    }
}
