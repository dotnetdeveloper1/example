using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Factories
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DefaultSubscriptionClientFactoryTests
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Create_WhenConnectionStringIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string connectionString) 
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                subscriptionClientFactory.Create(connectionString, topicName, subscriptionName, receiveMode, retryPolicy));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Create_WhenTopicNameIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string topicName)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                subscriptionClientFactory.Create(connectionString, topicName, subscriptionName, receiveMode, retryPolicy));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Create_WhenSubscriptionNameIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string subscriptionName)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                subscriptionClientFactory.Create(connectionString, topicName, subscriptionName, receiveMode, retryPolicy));
        }

        [TestMethod]
        public void Create_WhenRetryPolicyIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                subscriptionClientFactory.Create(connectionString, topicName, subscriptionName, receiveMode, null));
        }

        [TestMethod]
        public void Create_WhenValidParameters_ShouldCreateSubscriptionClient()
        {
            var subscriptionClient = subscriptionClientFactory.Create(connectionString, topicName, subscriptionName, receiveMode, retryPolicy);

            Assert.IsNotNull(subscriptionClient);
            Assert.IsInstanceOfType(subscriptionClient, typeof(SubscriptionClient));
        }

        private readonly DefaultSubscriptionClientFactory subscriptionClientFactory = new DefaultSubscriptionClientFactory();
        private readonly string connectionString = "Endpoint=sb://fake.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fakeAccessKey";
        private readonly string topicName = "TopicName";
        private readonly string subscriptionName = "DocumentAggregation.Service";
        private readonly ReceiveMode receiveMode = ReceiveMode.PeekLock;
        private readonly RetryPolicy retryPolicy = RetryPolicy.Default;
    }
}
