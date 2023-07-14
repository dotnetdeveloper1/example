using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Fakes;
using PWP.InvoiceCapture.Core.Telemetry;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MessageHandlerBaseTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            operationMock = mockRepository.Create<IOperation>();
            messageHandler = new FakeMessageHandler(telemetryClientMock.Object, applicationContextMock.Object);
            messageWithInheritanceHandler = new FakeMessageWithInheritanceHandler(telemetryClientMock.Object, applicationContextMock.Object);
            messageHandlerOperationName = messageHandler.GetType().Name;
            messageWithInheritanceHandlerOperationName = messageWithInheritanceHandler.GetType().Name;
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FakeMessageHandler(null, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FakeMessageHandler(telemetryClientMock.Object, null));
        }

        [TestMethod]
        public void HandleAsync_WhenMessageIsNull_ShouldThrowArgumentNullException()
        {
            SetupTelemetryClientMock(messageHandlerOperationName);

            Assert.ThrowsException<ArgumentNullException>(() => messageHandler.HandleAsync(null, cancellationToken));
        }

        [TestMethod]
        public void HandleAsync_WhenInnerMessageTypeIsIncorrect_ShouldThrowInvalidOperationException()
        {
            SetupTelemetryClientMock(messageHandlerOperationName);

            var innerMessage = "incorrectMessageType";
            var brokeredMessage = new BrokeredMessage { InnerMessage = innerMessage };

            Assert.ThrowsException<InvalidOperationException>(() => messageHandler.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsValid_ShouldStartTelemetryOperationAndHandleMessage()
        {
            SetupTelemetryClientMock(messageHandlerOperationName);

            var expectedInnerMessage = new FakeMessage();
            var expectedBrokeredMessage = new BrokeredMessage { InnerMessage = expectedInnerMessage };

            await messageHandler.HandleAsync(expectedBrokeredMessage, cancellationToken);

            Assert.AreEqual(expectedBrokeredMessage, messageHandler.ActualBrokeredMessage);
            Assert.AreEqual(expectedInnerMessage, messageHandler.ActualInnerMessage);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("default_tenant")]
        public async Task HandleAsync_WhenMultiTenantMessageIsValid_ShouldStartTelemetryOperationAndSetTenantIdAndHandleMessage(string tenantId)
        {
            SetupTelemetryClientMock(messageWithInheritanceHandlerOperationName);

            var expectedInnerMessage = new FakeMessageWithInheritance() { TenantId = tenantId };
            var expectedBrokeredMessage = new BrokeredMessage { InnerMessage = expectedInnerMessage };

            applicationContextMock
                .SetupSet(applicationContext => applicationContext.TenantId = tenantId)
                .Verifiable();

            await messageWithInheritanceHandler.HandleAsync(expectedBrokeredMessage, cancellationToken);

            Assert.AreEqual(expectedBrokeredMessage, messageWithInheritanceHandler.ActualBrokeredMessage);
            Assert.AreEqual(expectedInnerMessage, messageWithInheritanceHandler.ActualInnerMessage);
        }

        private void SetupTelemetryClientMock(string operationName) 
        {
            telemetryClientMock
                .Setup(telemetryClient => telemetryClient.StartOperation(operationName))
                .Returns(operationMock.Object);

            operationMock
                .Setup(operation => operation.Dispose())
                .Verifiable();
        }

        private Mock<IOperation> operationMock;
        private Mock<ITelemetryClient> telemetryClientMock;
        private Mock<IApplicationContext> applicationContextMock;
        private MockRepository mockRepository;
        private FakeMessageHandler messageHandler;
        private FakeMessageWithInheritanceHandler messageWithInheritanceHandler;
        private string messageHandlerOperationName;
        private string messageWithInheritanceHandlerOperationName;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
