using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Factories
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceBusSubscriberFactoryTests
    {
        [TestMethod]
        public void Instance_WhenManagementClientFactoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriberFactory(null, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, messageSerializerMock.Object));
        }

        [TestMethod]
        public void Instance_WhenSubscriptionClientFactoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriberFactory(managementClientFactoryMock.Object, null, messageTypeProviderMock.Object, messageSerializerMock.Object));
        }

        [TestMethod]
        public void Instance_WhenMessageTypeProviderIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriberFactory(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, null, messageSerializerMock.Object));
        }

        [TestMethod]
        public void Instance_WhenMessageSerializerIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriberFactory(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, null));
        }

        [TestMethod]
        public void Create_WhenMessageHandlersCollectionIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                factory.Create(null, exceptionHandlerMock.Object, serviceBusSubcriberOptions));
        }

        [TestMethod]
        public void Create_WhenMessageHandlersCollectionIsEmpty_ShouldThrowArgumentNullException()
        {
            var messageHandlers = new List<IMessageHandler>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                factory.Create(messageHandlers, exceptionHandlerMock.Object, serviceBusSubcriberOptions));
        }

        [TestMethod]
        public void Create_WhenExceptionHandlerIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                factory.Create(CreateMessageHandlers(), null, serviceBusSubcriberOptions));
        }

        [TestMethod]
        public void Create_WhenOptionsIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                factory.Create(CreateMessageHandlers(), exceptionHandlerMock.Object, null));
        }

        [TestMethod]
        public void Create_WhenValidParameters_ShouldCreateSubscriber()
        {
            var subscriber = factory.Create(CreateMessageHandlers(), exceptionHandlerMock.Object, serviceBusSubcriberOptions);

            Assert.IsNotNull(subscriber);
            Assert.IsInstanceOfType(subscriber, typeof(ServiceBusSubscriber));
        }

        private List<IMessageHandler> CreateMessageHandlers() 
        {
            var messageHandlerMock = new Mock<IMessageHandler>();

            messageHandlerMock
                .Setup(messageHandler => messageHandler.MessageType)
                .Returns(typeof(object));

            return new List<IMessageHandler> { messageHandlerMock.Object };
        }

        private readonly ServiceBusSubscriberFactory factory = new ServiceBusSubscriberFactory();
        private readonly Mock<IManagementClientFactory> managementClientFactoryMock = new Mock<IManagementClientFactory>();
        private readonly Mock<IMessageTypeProvider> messageTypeProviderMock = new Mock<IMessageTypeProvider>();
        private readonly Mock<IMessageSerializer> messageSerializerMock = new Mock<IMessageSerializer>();
        private readonly Mock<ISubscriptionClientFactory> subscriptionClientFactoryMock = new Mock<ISubscriptionClientFactory>();
        private readonly Mock<IExceptionHandler> exceptionHandlerMock = new Mock<IExceptionHandler>();
        private readonly ServiceBusSubscriberOptions serviceBusSubcriberOptions = new ServiceBusSubscriberOptions
        {
            ConnectionString = "Endpoint=sb://fake.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fakeAccessKey",
            TopicName = "TopicName",
            SubscriberName = "DocumentAggregation.Service"
        };
    }
}
