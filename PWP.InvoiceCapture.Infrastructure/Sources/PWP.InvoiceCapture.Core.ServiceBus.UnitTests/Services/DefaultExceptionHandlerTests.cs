using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using PWP.InvoiceCapture.Core.Telemetry;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DefaultExceptionHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            target = new DefaultExceptionHandler(telemetryClientMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DefaultExceptionHandler(null));
        }

        [TestMethod]
        public async Task HandleAsync_ShouldTrackException() 
        {
            var exception = new InvalidOperationException();
            var exceptionReceivedEventArgs = new ExceptionReceivedEventArgs(exception, "action", "endpoint", "entityName", "clientId");

            telemetryClientMock
                .Setup(telemetryClient => telemetryClient.TrackException(exception))
                .Verifiable();

            await target.HandleAsync(exceptionReceivedEventArgs);
        }

        private Mock<ITelemetryClient> telemetryClientMock;
        private MockRepository mockRepository;
        private DefaultExceptionHandler target;
    }
}
