using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Service.MessageHandlers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service.UnitTests.MessageHandlers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceStatusChangedMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            notificationServiceMock = mockRepository.Create<INotificationService>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            operationMock = mockRepository.Create<IOperation>();
            innerMessage = CreateMessage();
            brokeredMessage = new BrokeredMessage { InnerMessage = innerMessage };
            target = new InvoiceStatusChangedMessageHandler(notificationServiceMock.Object, telemetryClientMock.Object, applicationContextMock.Object);
            operationName = target.GetType().Name;
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenNotificationServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceStatusChangedMessageHandler(null, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceStatusChangedMessageHandler(notificationServiceMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceStatusChangedMessageHandler(notificationServiceMock.Object, telemetryClientMock.Object, null));
        }


        [TestMethod]
        public async Task HandleAsync_WhenMessageIsNull_ShouldThrowArgumentNullException()
        {
            SetupTelemetryClientMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.HandleAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task HandleAsync_WhenInnerMessageTypeIsIncorrect_ShouldThrowInvalidOperationException()
        {
            var innerMessage = "incorrectMessageType";
            var brokeredMessage = new BrokeredMessage { InnerMessage = innerMessage };

            SetupTelemetryClientMock();

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task HandleAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            innerMessage.InvoiceId = invoiceId;

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsValid_ShouldHandleMessage()
        {
            notificationServiceMock
                .Setup(notificationService => notificationService.NotifyAsync(invoiceId, cancellationToken))
                .Returns(Task.CompletedTask);

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await target.HandleAsync(brokeredMessage, cancellationToken);
        }

        private InvoiceStatusChangedMessage CreateMessage()
        {
            return new InvoiceStatusChangedMessage
            {
                InvoiceId = invoiceId
            };
        }

        private void SetupTelemetryClientMock()
        {
            telemetryClientMock
                .Setup(telemetryClient => telemetryClient.StartOperation(operationName))
                .Returns(operationMock.Object);

            operationMock
                .Setup(operation => operation.Dispose())
                .Verifiable();
        }

        private void SetupApplicationContextSetMock()
        {
            applicationContextMock
                .SetupSet(applicationContext => applicationContext.TenantId = null)
                .Verifiable();
        }

        private MockRepository mockRepository;
        private string operationName;
        private Mock<IOperation> operationMock;
        private Mock<ITelemetryClient> telemetryClientMock;
        private Mock<IApplicationContext> applicationContextMock;
        private Mock<INotificationService> notificationServiceMock;
        private BrokeredMessage brokeredMessage;
        private InvoiceStatusChangedMessage innerMessage;
        private InvoiceStatusChangedMessageHandler target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int invoiceId = 12;
    }
}
