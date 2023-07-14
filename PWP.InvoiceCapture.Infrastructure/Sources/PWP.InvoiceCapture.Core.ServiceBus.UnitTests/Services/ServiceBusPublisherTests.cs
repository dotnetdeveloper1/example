using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Fakes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceBusPublisherTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            managementClientFactoryMock = mockRepository.Create<IManagementClientFactory>();
            managementClientMock = mockRepository.Create<IManagementClient>();
            messageIdentityProviderMock = mockRepository.Create<IMessageIdentityProvider>();
            messageTypeProviderMock = mockRepository.Create<IMessageTypeProvider>();
            messageSerializerMock = mockRepository.Create<IMessageSerializer>();
            topicClientFactoryMock = mockRepository.Create<ITopicClientFactory>();
            topicClientMock = mockRepository.Create<ITopicClient>();
            options = CreateOptions();

            publisher = new ServiceBusPublisher(managementClientFactoryMock.Object, topicClientFactoryMock.Object, messageIdentityProviderMock.Object, 
                messageTypeProviderMock.Object, messageSerializerMock.Object, options);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenManagementClientFactoryIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new ServiceBusPublisher(null, topicClientFactoryMock.Object, messageIdentityProviderMock.Object, messageTypeProviderMock.Object, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public void Instance_WhenTopicClientFactoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisher(managementClientFactoryMock.Object, null, messageIdentityProviderMock.Object, messageTypeProviderMock.Object, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public void Instance_WhenMessageIdentityProviderIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisher(managementClientFactoryMock.Object, topicClientFactoryMock.Object, null, messageTypeProviderMock.Object, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public void Instance_WhenMessageTypeProviderIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisher(managementClientFactoryMock.Object, topicClientFactoryMock.Object, messageIdentityProviderMock.Object, null, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public void Instance_WhenMessageSerializerIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisher(managementClientFactoryMock.Object, topicClientFactoryMock.Object, messageIdentityProviderMock.Object, messageTypeProviderMock.Object, null, options));
        }

        [TestMethod]
        public void Instance_WhenOptionsIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisher(managementClientFactoryMock.Object, topicClientFactoryMock.Object, messageIdentityProviderMock.Object, messageTypeProviderMock.Object, messageSerializerMock.Object, null));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenConnectionStringIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string connectionString)
        {
            options.ConnectionString = connectionString;

            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisher(managementClientFactoryMock.Object, topicClientFactoryMock.Object, messageIdentityProviderMock.Object, messageTypeProviderMock.Object, messageSerializerMock.Object, options));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenTopicNameIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string topicName)
        {
            options.TopicName = topicName;

            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusPublisher(managementClientFactoryMock.Object, topicClientFactoryMock.Object, messageIdentityProviderMock.Object, messageTypeProviderMock.Object, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public async Task StartAsync_WhenManagementClientFactoryReturnsNull_ShouldThrowInvalidOperationException() 
        {
            managementClientFactoryMock
                .Setup(managementClientFactory => managementClientFactory.CreateAsync(options.ConnectionString, cancellationToken))
                .ReturnsAsync((IManagementClient)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publisher.StartAsync(cancellationToken));
        }

        [TestMethod]
        public async Task StartAsync_WhenManagementClientProvided_ShouldCreateOrUpdateTopicAndInstantiateTopicClient()
        {
            RetryPolicy actualRetryPolicy = null;

            SetupManagementClientFactoryMock();

            topicClientFactoryMock
                .Setup(topicClientFactory => topicClientFactory.Create(options.ConnectionString, options.TopicName, It.IsAny<RetryPolicy>()))
                .Callback<string, string, RetryPolicy>((connectionString, topicName, retryPolicy) => actualRetryPolicy = retryPolicy)
                .Returns(topicClientMock.Object);

            await publisher.StartAsync(cancellationToken);

            Assert.IsNotNull(actualRetryPolicy);
            Assert.IsInstanceOfType(actualRetryPolicy, typeof(RetryExponential));
        }

        [TestMethod]
        public async Task StopAsync_WhenStarted_ShouldStopTopicClient()
        {
            SetupManagementClientFactoryMock();
            SetupTopicClientFactoryMock();

            topicClientMock
                .Setup(topicClient => topicClient.CloseAsync())
                .Returns(Task.CompletedTask);

            await publisher.StartAsync(cancellationToken);
            await publisher.StopAsync(cancellationToken);
        }

        [TestMethod]
        public async Task StopAsync_WhenNotStarted_ShouldNotStopTopicClient()
        {
            await publisher.StopAsync(cancellationToken);
        }

        [TestMethod]
        public async Task PublishAsync_WhenMessageIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => publisher.PublishAsync<FakeMessage>(null, cancellationToken));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => publisher.PublishAsync<FakeMessage>(null, correlationId, cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task PublishAsync_WhenCorrelationIdIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string correlationId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => publisher.PublishAsync(fakeMessage, correlationId, cancellationToken));
        }

        [TestMethod]
        public async Task PublishAsync_WhenPublisherNotStarted_ShouldThrowInvalidOperationException()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publisher.PublishAsync(fakeMessage, cancellationToken));
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => publisher.PublishAsync(fakeMessage, correlationId, cancellationToken));
        }

        [TestMethod]
        public async Task PublishAsync_WhenPublisherStartedAndMessageIsNotNull_ShouldPublish()
        {
            Message actualMessage = null;

            SetupManagementClientFactoryMock();
            SetupTopicClientFactoryMock();
            SetupMessageSerializerMock();
            SetupMessageTypeProviderMock();
            SetupMessageIdentityProviderMock();

            topicClientMock
                .Setup(topicClient => topicClient.SendAsync(It.IsAny<Message>()))
                .Callback<Message>((message) => actualMessage = message)
                .Returns(Task.CompletedTask);

            await publisher.StartAsync(cancellationToken);
            await publisher.PublishAsync(fakeMessage, cancellationToken);

            Assert.IsNotNull(actualMessage);
            Assert.AreEqual(messageId, actualMessage.MessageId);
            Assert.IsNull(actualMessage.CorrelationId);
            Assert.AreEqual(messageType, actualMessage.Label);
            CollectionAssert.AreEqual(serializedMessageBytes, actualMessage.Body);
        }

        [TestMethod]
        public async Task PublishAsync_WhenPublisherStartedAndMessageIsNotNullAndCorrellationIdIsNotNull_ShouldPublish()
        {
            Message actualMessage = null;

            SetupManagementClientFactoryMock();
            SetupTopicClientFactoryMock();
            SetupMessageSerializerMock();
            SetupMessageTypeProviderMock();
            SetupMessageIdentityProviderMock();

            topicClientMock
                .Setup(topicClient => topicClient.SendAsync(It.IsAny<Message>()))
                .Callback<Message>((message) => actualMessage = message)
                .Returns(Task.CompletedTask);

            await publisher.StartAsync(cancellationToken);
            await publisher.PublishAsync(fakeMessage, correlationId, cancellationToken);

            Assert.IsNotNull(actualMessage);
            Assert.AreEqual(messageId, actualMessage.MessageId);
            Assert.AreEqual(correlationId, actualMessage.CorrelationId);
            Assert.AreEqual(messageType, actualMessage.Label);
            CollectionAssert.AreEqual(serializedMessageBytes, actualMessage.Body);
        }

        private void SetupMessageTypeProviderMock() 
        {
            messageTypeProviderMock
                .Setup(messageTypeProvider => messageTypeProvider.Get(typeof(FakeMessage)))
                .Returns(messageType);
        }

        private void SetupMessageIdentityProviderMock() 
        {
            messageIdentityProviderMock
                .Setup(messageIdentityProvider => messageIdentityProvider.GetNextAsync(cancellationToken))
                .ReturnsAsync(messageId);
        }

        private void SetupMessageSerializerMock() 
        {
            messageSerializerMock
                .Setup(messageSerializer => messageSerializer.Serialize(fakeMessage))
                .Returns(serializedMessageBytes);
        }

        private void SetupManagementClientFactoryMock()
        {
            managementClientFactoryMock
                .Setup(managementClientFactory => managementClientFactory.CreateAsync(options.ConnectionString, cancellationToken))
                .ReturnsAsync(managementClientMock.Object);

            managementClientMock
                .Setup(managementClient => managementClient.CreateOrUpdateTopicAsync(options.TopicName, cancellationToken))
                .Returns(Task.CompletedTask);
        }

        private void SetupTopicClientFactoryMock()
        {
            topicClientFactoryMock
                .Setup(topicClientFactory => topicClientFactory.Create(options.ConnectionString, options.TopicName, It.IsAny<RetryPolicy>()))
                .Returns(topicClientMock.Object);
        }

        private ServiceBusPublisherOptions CreateOptions()
        {
            return new ServiceBusPublisherOptions
            {
                ConnectionString = "Endpoint=sb://fake.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fakeAccessKey",
                TopicName = "TopicName"
            };
        }

        private MockRepository mockRepository;
        private Mock<IManagementClientFactory> managementClientFactoryMock;
        private Mock<IManagementClient> managementClientMock;
        private Mock<IMessageIdentityProvider> messageIdentityProviderMock;
        private Mock<IMessageTypeProvider> messageTypeProviderMock;
        private Mock<IMessageSerializer> messageSerializerMock;
        private Mock<ITopicClientFactory> topicClientFactoryMock;
        private Mock<ITopicClient> topicClientMock;
        private ServiceBusPublisher publisher;
        private ServiceBusPublisherOptions options;
        private readonly CancellationToken cancellationToken = new CancellationToken();
        private readonly string messageId = "messageId";
        private readonly string messageType = "messageType";
        private readonly string correlationId = "correlationId";
        private readonly byte[] serializedMessageBytes = new byte[] { 0x01, 0x02, 0x03 };
        private readonly FakeMessage fakeMessage = new FakeMessage();
    }
}
