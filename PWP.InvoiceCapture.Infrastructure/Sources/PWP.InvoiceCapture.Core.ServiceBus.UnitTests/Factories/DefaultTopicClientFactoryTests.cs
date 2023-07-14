using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.ServiceBus.Factories;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Factories
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DefaultTopicClientFactoryTests
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Create_WhenConnectionStringIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string connectionString)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                topicClientFactory.Create(connectionString, topicName, retryPolicy));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Create_WhenTopicNameIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string topicName)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                topicClientFactory.Create(connectionString, topicName, retryPolicy));
        }

        [TestMethod]
        public void Create_WhenRetryPolicyIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                topicClientFactory.Create(connectionString, topicName, null));
        }

        [TestMethod]
        public void Create_WhenValidParameters_ShouldCreateTopicClient()
        {
            var topicClient = topicClientFactory.Create(connectionString, topicName, retryPolicy);

            Assert.IsNotNull(topicClient);
            Assert.IsInstanceOfType(topicClient, typeof(TopicClient));
        }

        private readonly DefaultTopicClientFactory topicClientFactory = new DefaultTopicClientFactory();
        private readonly string connectionString = "Endpoint=sb://fake.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fakeAccessKey";
        private readonly string topicName = "TopicName";
        private readonly RetryPolicy retryPolicy = RetryPolicy.Default;
    }
}
