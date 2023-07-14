using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Factories
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceBusPublisherFactoryTests
    {
        [TestMethod]
        public void Instance_WhenManagementClientFactoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisherFactory(null, topicClientFactoryMock.Object, messageIdentityProviderMock.Object, messageTypeProviderMock.Object, messageSerializerMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTopicClientFactoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisherFactory(managementClientFactoryMock.Object, null, messageIdentityProviderMock.Object, messageTypeProviderMock.Object, messageSerializerMock.Object));
        }

        [TestMethod]
        public void Instance_WhenMessageIdentityProviderIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisherFactory(managementClientFactoryMock.Object, topicClientFactoryMock.Object, null, messageTypeProviderMock.Object, messageSerializerMock.Object));
        }

        [TestMethod]
        public void Instance_WhenMessageTypeProviderIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisherFactory(managementClientFactoryMock.Object, topicClientFactoryMock.Object, messageIdentityProviderMock.Object, null, messageSerializerMock.Object));
        }

        [TestMethod]
        public void Instance_WhenMessageSerializerIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisherFactory(managementClientFactoryMock.Object, topicClientFactoryMock.Object, messageIdentityProviderMock.Object, messageTypeProviderMock.Object, null));
        }

        [TestMethod]
        public void Create_WhenOptionsIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                factory.Create(null));
        }

        [TestMethod]
        public void Create_WhenValidParameters_ShouldCreatePublisher()
        {
            var publisher = factory.Create(serviceBusPublisherOptions);

            Assert.IsNotNull(publisher);
            Assert.IsInstanceOfType(publisher, typeof(ServiceBusPublisher));
        }

        private readonly ServiceBusPublisherFactory factory = new ServiceBusPublisherFactory();
        private readonly Mock<IManagementClientFactory> managementClientFactoryMock = new Mock<IManagementClientFactory>();
        private readonly Mock<IMessageIdentityProvider> messageIdentityProviderMock = new Mock<IMessageIdentityProvider>();
        private readonly Mock<IMessageTypeProvider> messageTypeProviderMock = new Mock<IMessageTypeProvider>();
        private readonly Mock<IMessageSerializer> messageSerializerMock = new Mock<IMessageSerializer>();
        private readonly Mock<ITopicClientFactory> topicClientFactoryMock = new Mock<ITopicClientFactory>();
        private readonly ServiceBusPublisherOptions serviceBusPublisherOptions = new ServiceBusPublisherOptions
        {
            ConnectionString = "Endpoint=sb://fake.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fakeAccessKey",
            TopicName = "TopicName"
        };
    }
}
