using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Service.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service.UnitTests.MessageHandlers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceDocumentUploadedMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceServiceMock = mockRepository.Create<IInvoiceService>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            operationMock = mockRepository.Create<IOperation>();
            serviceBusPublisherMock = mockRepository.Create<IServiceBusPublisher>();
            target = new InvoiceDocumentUploadedMessageHandler(invoiceServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object);
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
                new InvoiceDocumentUploadedMessageHandler(null, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenServiceBusPublisherIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceDocumentUploadedMessageHandler(invoiceServiceMock.Object, null, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceDocumentUploadedMessageHandler(invoiceServiceMock.Object, serviceBusPublisherMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceDocumentUploadedMessageHandler(invoiceServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, null));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsNull_ShouldThrowArgumentNullException()
        {
            SetupTelemetryClientMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => 
                target.HandleAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task HandleAsync_WhenFileIdIsNull_ShouldThrowArgumentNullException(string fileId)
        {
            var message = CreateMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            message.FileId = fileId;

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => 
                target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task HandleAsync_WhenFileNameIsNull_ShouldThrowArgumentNullException(string fileName)
        {
            var message = CreateMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            message.FileName = fileName;

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => 
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
        public async Task HandleAsync_WhenMessageIsValid_ShouldHandleMessage()
        {
            var message = CreateMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };
            var expectedInvoiceId = 12;
            var actualInvoiceId = 0;
            var actualInvoices = new List<Invoice>();
            var actualPublishedMessages = new List<InvoiceCreatedMessage>();

            SetupTelemetryClientMock();
            SetupApplicationContextGetMock();
            SetupApplicationContextSetMock();

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.CreateAsync(Capture.In(actualInvoices), cancellationToken))
                .Callback<Invoice, CancellationToken>((invoice, token) =>
                {
                    actualInvoiceId = invoice.Id;
                    invoice.Id = expectedInvoiceId;
                })
                .Returns(Task.CompletedTask);

            serviceBusPublisherMock
                .Setup(serviceBusPublisher => serviceBusPublisher.PublishAsync(Capture.In(actualPublishedMessages), cancellationToken))
                .Returns(Task.CompletedTask);

            await target.HandleAsync(brokeredMessage, cancellationToken);

            Assert.AreEqual(1, actualInvoices.Count);
            Assert.AreEqual(1, actualPublishedMessages.Count);

            var actualInvoice = actualInvoices.Single();
            var actualMessage = actualPublishedMessages.Single();

            Assert.IsNotNull(actualInvoice);
            Assert.AreEqual(0, actualInvoiceId);
            Assert.AreEqual(fileId, actualInvoice.FileId);
            Assert.AreEqual(fileName, actualInvoice.FileName);
            Assert.AreEqual(sourceType, actualInvoice.FileSourceType);
            Assert.AreEqual(InvoiceStatus.NotStarted, actualInvoice.Status);

            Assert.IsNotNull(actualMessage);
            Assert.AreEqual(expectedInvoiceId, actualMessage.InvoiceId);
            Assert.AreEqual(fileId, actualMessage.FileId);
            Assert.AreEqual(cultureUs, actualMessage.CultureName);
            Assert.AreEqual(tenantId, actualMessage.TenantId);
        }

        private InvoiceDocumentUploadedMessage CreateMessage() 
        {
            return new InvoiceDocumentUploadedMessage
            {
                FileId = fileId,
                FileName = fileName,
                FileSourceType = sourceType,
                TenantId = tenantId,
                CultureName = cultureUs
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

        private MockRepository mockRepository;
        private string operationName;
        private Mock<IOperation> operationMock;
        private Mock<ITelemetryClient> telemetryClientMock;
        private Mock<IApplicationContext> applicationContextMock;
        private Mock<IInvoiceService> invoiceServiceMock;
        private Mock<IServiceBusPublisher> serviceBusPublisherMock;
        private InvoiceDocumentUploadedMessageHandler target;
        private readonly string fileId = "4e461b57-4def-4862-899f-3c144a07cc65.pdf";
        private readonly string fileName = "invoice.pdf";
        private readonly FileSourceType sourceType = FileSourceType.API;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const string tenantId = "11";
        private const string cultureUs = "en-Us";
    }
}
