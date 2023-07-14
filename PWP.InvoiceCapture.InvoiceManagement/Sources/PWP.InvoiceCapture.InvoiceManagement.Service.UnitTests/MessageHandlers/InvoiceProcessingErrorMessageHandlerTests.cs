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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service.UnitTests.MessageHandlers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceProcessingErrorMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceServiceMock = mockRepository.Create<IInvoiceService>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            operationMock = mockRepository.Create<IOperation>();
            innerMessage = CreateMessage();
            brokeredMessage = new BrokeredMessage { InnerMessage = innerMessage };
            target = new InvoiceProcessingErrorMessageHandler(invoiceServiceMock.Object, telemetryClientMock.Object, applicationContextMock.Object);
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
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceProcessingErrorMessageHandler(null, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceProcessingErrorMessageHandler(invoiceServiceMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceProcessingErrorMessageHandler(invoiceServiceMock.Object, telemetryClientMock.Object, null));
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
            innerMessage.TenantId = tenantId;

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsValid_ShouldHandleMessage()
        {
            var actualExceptions = new List<Exception>();
            innerMessage.TenantId = tenantId;
            invoiceServiceMock
                .Setup(invoiceRepository => invoiceRepository.UpdateStatusAsync(invoiceId, InvoiceStatus.Error, cancellationToken))
                .Returns(Task.CompletedTask);
            invoiceServiceMock
                .Setup(invoiceRepository => invoiceRepository.PublishInvoiceStatusChangedMessageAsync(invoiceId, tenantId, cancellationToken))
                .Returns(Task.CompletedTask);

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            telemetryClientMock
               .Setup(telemetryClient => telemetryClient.TrackException(Capture.In(actualExceptions)))
               .Verifiable();

            telemetryClientMock
               .Setup(telemetryClient => telemetryClient.TrackTrace(innerMessage.Message))
               .Verifiable();

            await target.HandleAsync(brokeredMessage, cancellationToken);

            Assert.AreEqual(1, actualExceptions.Count);
            Assert.AreEqual(exceptionMessage, actualExceptions[0].Message);
        }

        private InvoiceProcessingErrorMessage CreateMessage()
        {
            return new InvoiceProcessingErrorMessage
            {
                InvoiceId = invoiceId, 
                TenantId = tenantId,
                Code = code,
                Exception = new Exception(exceptionMessage),
                Message = message
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
        private BrokeredMessage brokeredMessage;
        private InvoiceProcessingErrorMessage innerMessage;
        private InvoiceProcessingErrorMessageHandler target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const string tenantId = "11";
        private const int invoiceId = 22;
        private const string code = "testCode";
        private const string message = "testMessage";
        private const string exceptionMessage = "testExceptionMessage";
    }
}
