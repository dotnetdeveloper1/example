using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Fakes;
using PWP.InvoiceCapture.Core.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DefaultMessageSerializerTests
    {
        [TestMethod]
        public void Instance_WhenSerializationServiceIsNull_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DefaultMessageSerializer(null));
        }

        [TestMethod]
        public void Serialize_WhenObjectIsNull_ShouldReturnNull()
        {
            var actualBytes = messageSerializer.Serialize(null);

            Assert.IsNull(actualBytes);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(new byte[0])]
        public void Deserialize_WhenBufferIsNullOrEmpty_ShouldReturnNull(byte[] buffer)
        {
            var actualBytes = messageSerializer.Deserialize(buffer, typeof(FakeMessage));
            
            Assert.IsNull(actualBytes);
        }

        [TestMethod]
        public void Deserialize_WhenTypeIsNull_ShouldThrowArgumentNullException()
        {
            var buffer = messageSerializer.Serialize(fakeMessage);

            Assert.ThrowsException<ArgumentNullException>(() => messageSerializer.Deserialize(buffer, null));
        }

        [TestMethod]
        public void Deserialize_WhenPassingSerializedObject_ShouldReturnDeserializedObject()
        {
            var bytes = messageSerializer.Serialize(fakeMessage);

            Assert.IsNotNull(bytes);

            var expectedType = typeof(FakeMessage);
            var actualObject = messageSerializer.Deserialize(bytes, expectedType);

            Assert.IsNotNull(actualObject);
            Assert.IsInstanceOfType(actualObject, expectedType);

            var actualMessage = (FakeMessage)actualObject;

            Assert.AreEqual(fakeMessage.IntProperty, actualMessage.IntProperty);
            Assert.AreEqual(fakeMessage.StringProperty, actualMessage.StringProperty);

            // Ensure all properties are tested
            Assert.AreEqual(2, expectedType.GetProperties().Length);
        }

        private readonly FakeMessage fakeMessage = new FakeMessage
        {
            IntProperty = 1,
            StringProperty = "string"
        };

        private readonly DefaultMessageSerializer messageSerializer = new DefaultMessageSerializer(new SerializationService());
    }
}
