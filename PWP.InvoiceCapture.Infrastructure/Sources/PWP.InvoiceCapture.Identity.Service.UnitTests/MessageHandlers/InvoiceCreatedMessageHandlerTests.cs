using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.Identity.Service.MessageHandlers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Service.UnitTests.MessageHandlers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceCreatedMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            cancellationToken = CancellationToken.None;
            mockRepository = new MockRepository(MockBehavior.Strict);
            usageCalculationServiceMock = mockRepository.Create<IUsageService>();
            serviceBusPublisherMock = mockRepository.Create<IServiceBusPublisher>();
            operationMock = mockRepository.Create<IOperation>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            target = new InvoiceCreatedMessageHandler(usageCalculationServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object);
            operationName = target.GetType().Name;
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void HandlerInstance_WhenUsageCalculationServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceCreatedMessageHandler(null, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenServiceBusPublisherMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceCreatedMessageHandler(usageCalculationServiceMock.Object, null, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenTelemetryClientMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceCreatedMessageHandler(usageCalculationServiceMock.Object, serviceBusPublisherMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenApplicationContextMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceCreatedMessageHandler(usageCalculationServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, null));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            SetupTelemetryClientMock();
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.HandleAsync(null, cancellationToken));
        }

        [DataRow(null, 1)]
        [DataRow("", 2)]
        [DataRow("  ", 3)]
        [TestMethod]
        public async Task HandleAsync_WhenMessageFileIdIsNullOrWhitespace_ShouldThrowArgumentExceptionAsync(string fileId, int invoiceId)
        {
            var message = CreateInvoiceCreatedMessage(fileId, invoiceId, tenantId);
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [DataRow("fileId1", 0)]
        [DataRow("fileId2", -1)]
        [TestMethod]
        public async Task HandleAsync_WhenMessageInvoiceIdZeroOrLessZero_ShouldThrowArgumentExceptionAsync(string fileId, int invoiceId)
        {
            var message = CreateInvoiceCreatedMessage(fileId, invoiceId, tenantId);
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [DataRow("fileId1", 1)]
        [DataRow("fileId2", 2)]
        [DataRow("fileId3", 3)]
        [TestMethod]
        public async Task HandleAsync_WhenLimitNotExceeded_ShouldPublishMessageAsync(string fileId, int invoiceId)
        {
            var message = CreateInvoiceCreatedMessage(fileId, invoiceId, tenantId);
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextGetMock();
            SetupApplicationContextSetMock();

            usageCalculationServiceMock
                .Setup(usageCalculationService => usageCalculationService.TryIncreaseCountOfUploadedInvoicesAsync(Convert.ToInt32(tenantId), cancellationToken))
                .ReturnsAsync(true);

            var publishedMessages = new List<InvoiceProcessingLimitNotExceededMessage>();
            serviceBusPublisherMock
                .Setup(serviceBusPublisher => serviceBusPublisher.PublishAsync(Capture.In(publishedMessages), cancellationToken))
                .Returns(Task.CompletedTask);

            await target.HandleAsync(brokeredMessage, cancellationToken);

            Assert.AreEqual(publishedMessages.Count, 1);

            var publishedMessage = publishedMessages.FirstOrDefault();
            Assert.IsNotNull(publishedMessage);
            Assert.AreEqual(publishedMessage.FileId, message.FileId);
            Assert.AreEqual(publishedMessage.InvoiceId, message.InvoiceId);
            Assert.AreEqual(publishedMessage.TenantId, message.TenantId);
        }

        [DataRow("fileId1", 1)]
        [DataRow("fileId2", 2)]
        [DataRow("fileId3", 3)]
        [TestMethod]
        public async Task HandleAsync_WhenLimitExceeded_ShouldPublishMessageAsync(string fileId, int invoiceId)
        {
            var message = CreateInvoiceCreatedMessage(fileId, invoiceId, tenantId);
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextGetMock();
            SetupApplicationContextSetMock();

            usageCalculationServiceMock
                .Setup(usageCalculationService => usageCalculationService.TryIncreaseCountOfUploadedInvoicesAsync(Convert.ToInt32(tenantId), cancellationToken))
                .ReturnsAsync(false);

            var publishedMessages = new List<InvoiceProcessingLimitExceededMessage>();
            serviceBusPublisherMock
                .Setup(serviceBusPublisher => serviceBusPublisher.PublishAsync(Capture.In(publishedMessages), cancellationToken))
                .Returns(Task.CompletedTask);

            await target.HandleAsync(brokeredMessage, cancellationToken);

            Assert.AreEqual(publishedMessages.Count, 1);

            var publishedMessage = publishedMessages.FirstOrDefault();
            Assert.IsNotNull(publishedMessage);
            Assert.AreEqual(publishedMessage.FileId, message.FileId);
            Assert.AreEqual(publishedMessage.InvoiceId, message.InvoiceId);
            Assert.AreEqual(publishedMessage.TenantId, message.TenantId);
        }

        private InvoiceCreatedMessage CreateInvoiceCreatedMessage(string fileId, int invoiceId, string tenantId)
        {
            return new InvoiceCreatedMessage
            {
                FileId = fileId,
                InvoiceId = invoiceId,
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

        private InvoiceCreatedMessageHandler target;
        private MockRepository mockRepository;
        private Mock<ITelemetryClient> telemetryClientMock;
        private Mock<IApplicationContext> applicationContextMock;
        private Mock<IServiceBusPublisher> serviceBusPublisherMock;
        private Mock<IUsageService> usageCalculationServiceMock;
        private Mock<IOperation> operationMock;
        private CancellationToken cancellationToken;
        private string operationName;

        private const string tenantId = "1";
        private const string cultureUs = "en-Us";
    }
}
