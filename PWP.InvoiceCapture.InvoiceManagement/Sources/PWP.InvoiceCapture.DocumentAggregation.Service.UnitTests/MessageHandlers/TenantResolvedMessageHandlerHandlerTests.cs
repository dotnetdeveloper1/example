using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.DocumentAggregation.Service.MessageHandlers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.Service.UnitTests.MessageHandlers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TenantResolvedMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            operationMock = mockRepository.Create<IOperation>();
            serviceBusPublisherMock = mockRepository.Create<IServiceBusPublisher>();
            target = new TenantEmailResolvedMessageHandler(serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object);
            operationName = target.GetType().Name;
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenServiceBusPublisherIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new TenantEmailResolvedMessageHandler(null, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTelemetryClientMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new TenantEmailResolvedMessageHandler(serviceBusPublisherMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new TenantEmailResolvedMessageHandler(serviceBusPublisherMock.Object, telemetryClientMock.Object, null));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            SetupTelemetryClientMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => 
                target.HandleAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task HandleAsync_WhenFileIdIsNull_ShouldThrowArgumentNullExceptionAsync(string fileId)
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
        public async Task HandleAsync_WhenFileNameIsNull_ShouldThrowArgumentNullExceptionAsync(string fileName)
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
        public async Task HandleAsync_WhenInnerMessageTypeIsIncorrect_ShouldThrowInvalidOperationExceptionAsync()
        {
            var innerMessage = "incorrectMessageType";
            var brokeredMessage = new BrokeredMessage { InnerMessage = innerMessage };

            SetupTelemetryClientMock();

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => 
                target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsValid_ShouldHandleMessageAsync()
        {
            var message = CreateMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };
            var actualPublishedMessages = new List<InvoiceDocumentUploadedMessage>();

            SetupTelemetryClientMock();
            SetupApplicationContextGetMock();
            SetupApplicationContextSetMock();

            serviceBusPublisherMock
                .Setup(serviceBusPublisher => serviceBusPublisher.PublishAsync(Capture.In(actualPublishedMessages), cancellationToken))
                .Returns(Task.CompletedTask);

            await target.HandleAsync(brokeredMessage, cancellationToken);

            Assert.AreEqual(1, actualPublishedMessages.Count);

            var actualMessage = actualPublishedMessages.Single();

            Assert.IsNotNull(actualMessage);
            Assert.AreEqual(fileId, actualMessage.FileId);
            Assert.AreEqual(tenantId, actualMessage.TenantId);
            Assert.AreEqual(fileName, actualMessage.FileName);
            Assert.AreEqual(sourceType, actualMessage.FileSourceType);
            Assert.AreEqual(fromEmail, actualMessage.FromEmailAddress);
            Assert.AreEqual(cultureUs, actualMessage.CultureName);
        }

        private TenantEmailResolvedMessage CreateMessage() 
        {
            return new TenantEmailResolvedMessage
            {
                FileId = fileId,
                FileName = fileName,
                FileSourceType = sourceType,
                TenantId = tenantId,
                From = fromEmail, 
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
        private Mock<IServiceBusPublisher> serviceBusPublisherMock;
        private TenantEmailResolvedMessageHandler target;
        private readonly string fileId = "4e461b57-4def-4862-899f-3c144a07cc65.pdf";
        private readonly string fileName = "invoice.pdf";
        private readonly FileSourceType sourceType = FileSourceType.Email;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const string tenantId = "11";
        private const string fromEmail = "email@email.com";
        private const string cultureUs = "en-Us";
    }
}
