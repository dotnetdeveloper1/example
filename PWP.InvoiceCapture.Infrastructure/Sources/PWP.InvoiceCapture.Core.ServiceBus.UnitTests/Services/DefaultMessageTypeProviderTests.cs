using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Fakes;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DefaultMessageTypeProviderTests
    {
        [TestMethod]
        public void Get_WhenMessageTypeIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => messageTypeProvider.Get(null));
        }

        [TestMethod]
        [DataRow(typeof(FakeMessage), "fakemessage")]
        [DataRow(typeof(Array), "array")]
        [DataRow(typeof(string), "string")]
        public void Get_WhenMessageTypeIsProvided_ShouldReturnTypeNameInLowerCase(Type type, string expectedMessageType)
        {
            var actualMessageType = messageTypeProvider.Get(type);

            Assert.IsFalse(string.IsNullOrWhiteSpace(actualMessageType));
            Assert.AreEqual(expectedMessageType, actualMessageType);
        }

        private readonly DefaultMessageTypeProvider messageTypeProvider = new DefaultMessageTypeProvider();
    }
}
