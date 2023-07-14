using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
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
    public class InvoiceProcessingStartedMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceServiceMock = mockRepository.Create<IInvoiceService>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            operationMock = mockRepository.Create<IOperation>();
            target = new InvoiceProcessingStartedMessageHandler(invoiceServiceMock.Object, telemetryClientMock.Object, applicationContextMock.Object);
            operationName = target.GetType().Name;
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoiceServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new InvoiceProcessingStartedMessageHandler(null, telemetryClientMock.Object, applicationContextMock.Object));
        }
       
        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingStartedMessageHandler(invoiceServiceMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingStartedMessageHandler(invoiceServiceMock.Object, telemetryClientMock.Object, null));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsNull_ShouldThrowArgumentNullException()
        {
            SetupTelemetryClientMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => 
                target.HandleAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task HandleAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentNullException(int invoiceId)
        {
            var message = CreateMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            message.InvoiceId = invoiceId;

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => 
                target.HandleAsync(brokeredMessage, cancellationToken));
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
        [DataRow(InvoiceStatus.NotStarted)]
        [DataRow(InvoiceStatus.Queued)]
        public async Task HandleAsync_WhenMessageIsValid_ShouldHandleMessage(InvoiceStatus status)
        {
            var message = CreateMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };
            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.LockAsync(invoiceId, cancellationToken))
                .Returns(Task.CompletedTask);
            
            invoiceServiceMock
                .Setup(invoiceRepository => invoiceRepository.PublishInvoiceStatusChangedMessageAsync(invoiceId, tenantId, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.UpdateStatusAsync(invoiceId, InvoiceStatus.InProgress, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetAsync(invoiceId, cancellationToken))
                .Returns(Task.FromResult(new Invoice() { Status = status }));

            await target.HandleAsync(brokeredMessage, cancellationToken);
        }

        [DataRow(InvoiceStatus.InProgress)]
        [DataRow(InvoiceStatus.PendingReview)]
        [DataRow(InvoiceStatus.Completed)]
        [TestMethod]
        public async Task HandleAsync_WhenStatusIsGreaterThenInProgress_ShouldNotChangeStatus(InvoiceStatus status)
        {
            var message = CreateMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };
            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.LockAsync(invoiceId, cancellationToken))
                .Returns(Task.CompletedTask);

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetAsync(invoiceId, cancellationToken))
                .Returns(Task.FromResult(new Invoice() {Status = status }));

            await target.HandleAsync(brokeredMessage, cancellationToken);
        }

        private InvoiceProcessingStartedMessage CreateMessage() 
        {
            return new InvoiceProcessingStartedMessage
            {
                InvoiceId = invoiceId,
                TenantId = tenantId
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
                .SetupSet(applicationContext => applicationContext.TenantId = tenantId)
                .Verifiable();
        }

        private MockRepository mockRepository;
        private string operationName;
        private Mock<IOperation> operationMock;
        private Mock<ITelemetryClient> telemetryClientMock;
        private Mock<IApplicationContext> applicationContextMock;
        private Mock<IInvoiceService> invoiceServiceMock;
        private InvoiceProcessingStartedMessageHandler target;
        private readonly int invoiceId = 2;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const string tenantId = "11";
    }
}
