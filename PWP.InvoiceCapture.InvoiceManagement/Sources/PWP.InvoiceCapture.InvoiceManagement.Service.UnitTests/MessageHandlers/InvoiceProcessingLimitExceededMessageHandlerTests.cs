using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
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
    public class InvoiceProcessingLimitExceededMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            cancellationToken = CancellationToken.None;
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceServiceMock = mockRepository.Create<IInvoiceService>();
            operationMock = mockRepository.Create<IOperation>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            target = new InvoiceProcessingLimitExceededMessageHandler(invoiceServiceMock.Object, telemetryClientMock.Object, applicationContextMock.Object);
            operationName = target.GetType().Name;
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void HandlerInstance_WhenInvoiceServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingLimitExceededMessageHandler(null, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingLimitExceededMessageHandler(invoiceServiceMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingLimitExceededMessageHandler(invoiceServiceMock.Object, telemetryClientMock.Object, null));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsNull_ShouldThrowArgumentNullException()
        {
            SetupTelemetryClientMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.HandleAsync(null, cancellationToken));
        }

        [DataRow("1", 0)]
        [DataRow("2", -1)]
        [TestMethod]
        public async Task HandleAsync_WhenMessageInvoiceIdZeroOrLessZero_ShouldThrowArgumentException(string fileId, int invoiceId)
        {
            var message = CreateInvoiceProcessingLimitExceededMessage(fileId, invoiceId);
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [DataRow("1", 1)]
        [DataRow("2", 2)]
        [DataRow("3", 3)]
        [TestMethod]
        public async Task HandleAsync_WhenMessageIsCorrect_ShouldPublishMessage(string fileId, int invoiceId)
        {
            var message = CreateInvoiceProcessingLimitExceededMessage(fileId, invoiceId);
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.UpdateStatusAsync(invoiceId, InvoiceStatus.LimitExceeded, cancellationToken))
                .Returns(Task.CompletedTask);
            invoiceServiceMock
                .Setup(invoiceRepository => invoiceRepository.PublishInvoiceStatusChangedMessageAsync(invoiceId, tenantId, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.HandleAsync(brokeredMessage, cancellationToken);
        }

        private InvoiceProcessingLimitExceededMessage CreateInvoiceProcessingLimitExceededMessage(string fileId, int invoiceId)
        {
            return new InvoiceProcessingLimitExceededMessage
            {
                FileId = fileId,
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

        private void SetupApplicationContextGetMock()
        {
            applicationContextMock
                .SetupGet(applicationContext => applicationContext.TenantId)
                .Returns(tenantId);
        }

        private void SetupApplicationContextSetMock()
        {
            applicationContextMock
                .SetupSet(applicationContext => applicationContext.TenantId = tenantId)
                .Verifiable();
        }

        private InvoiceProcessingLimitExceededMessageHandler target;
        private MockRepository mockRepository;
        private Mock<IOperation> operationMock;
        private Mock<ITelemetryClient> telemetryClientMock;
        private Mock<IApplicationContext> applicationContextMock;
        private Mock<IInvoiceService> invoiceServiceMock;
        private CancellationToken cancellationToken;
        private string operationName;

        private const string tenantId = "1";
    }
}
