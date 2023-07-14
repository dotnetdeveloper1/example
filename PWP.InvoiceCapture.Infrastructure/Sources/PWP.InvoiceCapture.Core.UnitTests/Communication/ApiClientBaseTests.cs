using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.UnitTests.Communication
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ApiClientBaseTests
    {
        [TestInitialize]
        public void Initialize()
        {
            options = new ApiClientOptions
            {
                BaseAddress = "http://localhost:10083",
                RetryAttemptCount = 5,
                TimeoutInSeconds = 30
            };

            target = new ApiClient(options);
        }

        [TestMethod]
        public void Instance_WhenApiClientOptionsAreValid_ShouldCreateInstanceAndSetupHttpClientAndRetryPolicies()
        {
            var client = new ApiClient(options);

            Assert.IsNotNull(client);

            Assert.IsNotNull(client.HttpClient);
            Assert.AreEqual(new Uri(options.BaseAddress), client.HttpClient.BaseAddress);
            Assert.AreEqual(TimeSpan.FromSeconds(options.TimeoutInSeconds), client.HttpClient.Timeout);

            Assert.IsNotNull(client.HttpRetryPolicy);
            Assert.IsNotNull(client.RetryPolicy);
        }

        [TestMethod]
        public void Instance_WhenApiClientOptionsAreNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApiClient(null));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenBaseAddressIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string baseAddress)
        {
            options.BaseAddress = baseAddress;

            Assert.ThrowsException<ArgumentNullException>(() => new ApiClient(options));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public void Instance_WhenRetryAttemptCountIsZeroOrNegative_ShouldThrowArgumentException(int retryAttemptCount)
        {
            options.RetryAttemptCount = retryAttemptCount;

            Assert.ThrowsException<ArgumentException>(() => new ApiClient(options));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public void Instance_WhenTimeoutInSecondsIsZeroOrNegative_ShouldThrowArgumentException(int timeoutInSeconds)
        {
            options.TimeoutInSeconds = timeoutInSeconds;

            Assert.ThrowsException<ArgumentException>(() => new ApiClient(options));
        }

        [TestMethod]
        [DataRow(1, 2)]
        [DataRow(2, 4)]
        [DataRow(3, 8)]
        [DataRow(4, 16)]
        [DataRow(5, 32)]
        [DataRow(6, 64)]
        public void GetWaitInterval_ShouldIncrementDurationExponentially(int attempt, int timeoutInSeconds) 
        {
            var expectedDelay = TimeSpan.FromSeconds(timeoutInSeconds);
            var actualDelay = target.GetWaitInterval(attempt);

            Assert.AreEqual(expectedDelay, actualDelay);
        }

        private ApiClientOptions options;
        private ApiClient target;
    }
}
