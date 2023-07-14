using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Fakes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceBusSubscriberTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            managementClientFactoryMock = mockRepository.Create<IManagementClientFactory>();
            managementClientMock = mockRepository.Create<IManagementClient>();
            messageTypeProviderMock = mockRepository.Create<IMessageTypeProvider>();
            messageSerializerMock = mockRepository.Create<IMessageSerializer>();
            subscriptionClientFactoryMock = mockRepository.Create<ISubscriptionClientFactory>();
            subscriptionClientMock = mockRepository.Create<ISubscriptionClient>();
            exceptionHandlerMock = mockRepository.Create<IExceptionHandler>();
            messageHandlerMock = mockRepository.Create<IMessageHandler>();
            options = CreateOptions();
            messageHandlers = CreateMessageHandlersAndSetupTypeProvider();

            subscriber = new ServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object,
                exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options);
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
                new ServiceBusSubscriber(null, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public void Instance_WhenSubscriptionClientFactoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriber(managementClientFactoryMock.Object, null, messageTypeProviderMock.Object, exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public void Instance_WhenMessageTypeProviderIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, null, exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public void Instance_WhenExceptionHandlerIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, null, messageHandlers, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public void Instance_WhenMessageHandlersCollectionIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, exceptionHandlerMock.Object, null, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public void Instance_WhenMessageHandlersCollectionIsEmpty_ShouldThrowArgumentNullException()
        {
            var messageHandlers = new List<IMessageHandler>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public void Instance_WhenMessageSerializerIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, exceptionHandlerMock.Object, messageHandlers, null, options));
        }

        [TestMethod]
        public void Instance_WhenOptionsIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, null));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenConnectionStringIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string connectionString)
        {
            options.ConnectionString = connectionString;

            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenTopicNameIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string topicName)
        {
            options.TopicName = topicName;

            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenSubscriberNameIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string subscriberName)
        {
            options.SubscriberName = subscriberName;

            Assert.ThrowsException<ArgumentNullException>(() =>
                new ServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object, exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options));
        }

        [TestMethod]
        public async Task StartAsync_WhenManagementClientFactoryReturnsNull_ShouldThrowInvalidOperationException()
        {
            managementClientFactoryMock
                .Setup(managementClientFactory => managementClientFactory.CreateAsync(options.ConnectionString, cancellationToken))
                .ReturnsAsync((IManagementClient)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => subscriber.StartAsync(cancellationToken));
        }

        [TestMethod]
        public async Task StartAsync_WhenManagementClientProvided_ShouldCreateOrUpdateTopicAndSubcriptionAndInstantiateSubscriptionClientAndRegisterMessageHandler()
        {
            RetryPolicy actualRetryPolicy = null;
            MessageHandlerOptions actualMessageHandlerOptions = null;

            SetupManagementClientFactoryMock();

            subscriptionClientFactoryMock
                .Setup(topicClientFactory => topicClientFactory.Create(options.ConnectionString, options.TopicName, options.SubscriberName, ReceiveMode.PeekLock, It.IsAny<RetryPolicy>()))
                .Callback<string, string, string, ReceiveMode, RetryPolicy>((connectionString, topicName, subscriptionName, receriveMode, retryPolicy) => actualRetryPolicy = retryPolicy)
                .Returns(subscriptionClientMock.Object);

            subscriptionClientMock
                .Setup(subscriptionClient => subscriptionClient.RegisterMessageHandler(It.IsAny<Func<Message, CancellationToken, Task>>(), It.IsAny<MessageHandlerOptions>()))
                .Callback<Func<Message, CancellationToken, Task>, MessageHandlerOptions>((handler, handlerOptions) => actualMessageHandlerOptions = handlerOptions)
                .Verifiable();

            await subscriber.StartAsync(cancellationToken);

            Assert.IsNotNull(actualRetryPolicy);
            Assert.IsInstanceOfType(actualRetryPolicy, typeof(RetryExponential));

            Assert.IsNotNull(actualMessageHandlerOptions);
            Assert.AreEqual(options.MaxConcurrentCalls, actualMessageHandlerOptions.MaxConcurrentCalls);
            Assert.AreEqual(exceptionHandlerMock.Object.HandleAsync, actualMessageHandlerOptions.ExceptionReceivedHandler);
            Assert.IsFalse(actualMessageHandlerOptions.AutoComplete);
        }

        [TestMethod]
        public async Task StartAsync_WhenManagementClientProvidedAndMessageTypesChanged_ShouldUpdateMessageTypes()
        {
            List<string> actualDeletedRuleNames = null;
            List<RuleDescription> actualAddedRules = null;
            
            var existingRuleDescriptions = new List<RuleDescription> 
            { 
                CreateRuleDescription("messageTypeToDelete1"),
                CreateRuleDescription("messageTypeToDelete2")
            };

            SetupManagementClientFactoryMock();
            SetupSubcriptionClientFactoryMock();

            managementClientMock
                .Setup(managementClient => managementClient.GetRulesAsync(options.TopicName, options.SubscriberName, cancellationToken))
                .ReturnsAsync(existingRuleDescriptions);

            managementClientMock
                .Setup(managementClient => managementClient.DeleteRulesAsync(options.TopicName, options.SubscriberName, It.IsAny<IEnumerable<string>>(), cancellationToken))
                .Callback<string, string, IEnumerable<string>, CancellationToken>((topic, subscriber, ruleNames, cancellationToken) => actualDeletedRuleNames = ruleNames.ToList())
                .Returns(Task.CompletedTask);

            managementClientMock
                .Setup(managementClient => managementClient.CreateRulesAsync(options.TopicName, options.SubscriberName, It.IsAny<IEnumerable<RuleDescription>>(), cancellationToken))
                .Callback<string, string, IEnumerable<RuleDescription>, CancellationToken>((topic, subscriber, rules, cancellationToken) => actualAddedRules = rules.ToList())
                .Returns(Task.CompletedTask);

            await subscriber.StartAsync(cancellationToken);

            Assert.IsNotNull(actualDeletedRuleNames);

            var deletedRuleNames = existingRuleDescriptions.Select(rule => rule.Name).ToList();
            CollectionAssert.AreEqual(deletedRuleNames, actualDeletedRuleNames);

            Assert.IsNotNull(actualAddedRules);
            Assert.AreEqual(1, actualAddedRules.Count);
            AssertRegisteredRule(actualAddedRules.First(), messageType);
        }

        [TestMethod]
        public async Task StopAsync_WhenStarted_ShouldStopSubscriptionClient()
        {
            SetupManagementClientFactoryMock();
            SetupSubcriptionClientFactoryMock();

            subscriptionClientMock
                .Setup(subscriptionClient => subscriptionClient.CloseAsync())
                .Returns(Task.CompletedTask);

            await subscriber.StartAsync(cancellationToken);
            await subscriber.StopAsync(cancellationToken);
        }

        [TestMethod]
        public async Task StopAsync_WhenNotStarted_ShouldNotStopSubscriptionClient()
        {
            await subscriber.StopAsync(cancellationToken);
        }

        [TestMethod]
        public async Task ProcessMessageAsync_WhenCancellationIsRequested_ShouldAbandonProcessing() 
        {
            var subscriber = new FakeServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object,
                exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options);

            var message = new Message();
            var cancelledToken = new CancellationToken(true);

            SetupManagementClientFactoryMock();
            SetupSubcriptionClientFactoryMock();

            subscriptionClientMock
                .Setup(subscriptionClient => subscriptionClient.AbandonAsync(null, null))
                .Returns(Task.CompletedTask);

            await subscriber.StartAsync(cancellationToken);
            await subscriber.ProcessMessagesAsync(message, cancelledToken);
        }

        [TestMethod]
        public async Task ProcessMessageAsync_WhenMessageHandlerIsMissing_ShouldThrowInvalidOperationException()
        {
            var subscriber = new FakeServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object,
                exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options);

            var message = new Message() 
            {
                Label = "UnknownMessageType"
            };

            SetupManagementClientFactoryMock();
            SetupSubcriptionClientFactoryMock();

            await subscriber.StartAsync(cancellationToken);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => subscriber.ProcessMessagesAsync(message, cancellationToken));
        }

        [TestMethod]
        public async Task ProcessMessageAsync_WhenMessageHandlerFound_ShouldHandleAndCompleteMessage()
        {
            BrokeredMessage actualBrokeredMessage = null;

            var subscriber = new FakeServiceBusSubscriber(managementClientFactoryMock.Object, subscriptionClientFactoryMock.Object, messageTypeProviderMock.Object,
                exceptionHandlerMock.Object, messageHandlers, messageSerializerMock.Object, options);

            var message = new Message()
            {
                MessageId = messageId,
                CorrelationId = correlationId,
                Label = messageType,
                Body = serializedMessageBytes
            };

            SetupManagementClientFactoryMock();
            SetupSubcriptionClientFactoryMock();
            SetupMessageSerializerMock();

            messageHandlerMock
                .Setup(messageHandler => messageHandler.HandleAsync(It.IsAny<BrokeredMessage>(), cancellationToken))
                .Callback<BrokeredMessage, CancellationToken>((brokeredMessage, cancellationToken) => actualBrokeredMessage = brokeredMessage)
                .Returns(Task.CompletedTask);

            subscriptionClientMock
                .Setup(subscriptionClient => subscriptionClient.CompleteAsync(null))
                .Returns(Task.CompletedTask);

            await subscriber.StartAsync(cancellationToken);
            await subscriber.ProcessMessagesAsync(message, cancellationToken);

            Assert.IsNotNull(actualBrokeredMessage);
            Assert.AreEqual(message.MessageId, actualBrokeredMessage.Id);
            Assert.AreEqual(message.CorrelationId, actualBrokeredMessage.CorrelationId);
            Assert.AreEqual(message.Label, actualBrokeredMessage.MessageType);
            Assert.AreSame(fakeMessage, actualBrokeredMessage.InnerMessage);
        }

        private void SetupManagementClientFactoryMock() 
        {
            managementClientFactoryMock
                .Setup(managementClientFactory => managementClientFactory.CreateAsync(options.ConnectionString, cancellationToken))
                .ReturnsAsync(managementClientMock.Object);

            managementClientMock
                .Setup(managementClient => managementClient.CreateOrUpdateTopicAsync(options.TopicName, cancellationToken))
                .Returns(Task.CompletedTask);

            managementClientMock
                .Setup(managementClient => managementClient.CreateOrUpdateSubsciptionAsync(options.TopicName, options.SubscriberName, options.MessageLockDuration, cancellationToken))
                .Returns(Task.CompletedTask);

            var existingRuleDescriptions = new List<RuleDescription> { CreateRuleDescription(messageType) };

            managementClientMock
                .Setup(managementClient => managementClient.GetRulesAsync(options.TopicName, options.SubscriberName, cancellationToken))
                .ReturnsAsync(existingRuleDescriptions);
        }

        private void SetupSubcriptionClientFactoryMock()
        {
            subscriptionClientFactoryMock
                .Setup(subscriptionClientFactory => subscriptionClientFactory.Create(options.ConnectionString, options.TopicName, options.SubscriberName, ReceiveMode.PeekLock, It.IsAny<RetryPolicy>()))
                .Returns(subscriptionClientMock.Object);

            subscriptionClientMock
                .Setup(subscriptionClient => subscriptionClient.RegisterMessageHandler(It.IsAny<Func<Message, CancellationToken, Task>>(), It.IsAny<MessageHandlerOptions>()))
                .Verifiable();
        }

        private void SetupMessageSerializerMock()
        {
            messageSerializerMock
                .Setup(messageSerializer => messageSerializer.Deserialize(serializedMessageBytes, typeof(FakeMessage)))
                .Returns(fakeMessage);
        }

        private void AssertRegisteredRule(RuleDescription rule, string messageType) 
        {
            Assert.IsNotNull(rule);
            Assert.AreEqual(messageType, rule.Name);

            Assert.IsNotNull(rule.Filter);
            Assert.IsInstanceOfType(rule.Filter, typeof(CorrelationFilter));

            var filter = (CorrelationFilter)rule.Filter;

            Assert.AreEqual(messageType, filter.Label);
        }

        private ServiceBusSubscriberOptions CreateOptions()
        {
            return new ServiceBusSubscriberOptions
            {
                ConnectionString = "Endpoint=sb://fake.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fakeAccessKey",
                TopicName = "TopicName",
                SubscriberName = "DocumentAggregation.Service",
                MaxConcurrentCalls = 10,
                MessageLockDuration = TimeSpan.FromSeconds(30)
            };
        }

        private List<IMessageHandler> CreateMessageHandlersAndSetupTypeProvider() 
        {
            messageHandlerMock
                .Setup(messageHandler => messageHandler.MessageType)
                .Returns(typeof(FakeMessage));

            messageTypeProviderMock
                .Setup(messageTypeProvider => messageTypeProvider.Get(typeof(FakeMessage)))
                .Returns(messageType);

            return new List<IMessageHandler> { messageHandlerMock.Object };
        }

        private RuleDescription CreateRuleDescription(string messageType) 
        {
            var filter = new CorrelationFilter { Label = messageType };

            return new RuleDescription(messageType, filter);
        }

        private MockRepository mockRepository;
        private Mock<IManagementClientFactory> managementClientFactoryMock;
        private Mock<IManagementClient> managementClientMock;
        private Mock<IMessageTypeProvider> messageTypeProviderMock;
        private Mock<IMessageSerializer> messageSerializerMock;
        private Mock<ISubscriptionClientFactory> subscriptionClientFactoryMock;
        private Mock<ISubscriptionClient> subscriptionClientMock;
        private Mock<IExceptionHandler> exceptionHandlerMock;
        private Mock<IMessageHandler> messageHandlerMock;
        private ServiceBusSubscriber subscriber;
        private ServiceBusSubscriberOptions options;
        private List<IMessageHandler> messageHandlers;
        private readonly CancellationToken cancellationToken = new CancellationToken();
        private readonly byte[] serializedMessageBytes = new byte[] { 0x01, 0x02, 0x03 };
        private readonly FakeMessage fakeMessage = new FakeMessage();
        private readonly string messageType = "messageType";
        private readonly string messageId = "messageId";
        private readonly string correlationId = "correlationId";
    }
}
