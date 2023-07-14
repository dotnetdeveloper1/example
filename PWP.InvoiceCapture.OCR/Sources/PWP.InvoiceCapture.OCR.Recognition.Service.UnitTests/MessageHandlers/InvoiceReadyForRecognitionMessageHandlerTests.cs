using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Mappers;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using PWP.InvoiceCapture.OCR.Recognition.Service.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Service.UnitTests.MessageHandlers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceReadyForRecognitionMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            cancellationToken = CancellationToken.None;
            mockRepository = new MockRepository(MockBehavior.Strict);
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            operationMock = mockRepository.Create<IOperation>();
            serviceBusPublisherMock = mockRepository.Create<IServiceBusPublisher>();
            recognitionEngineMock = mockRepository.Create<IRecognitionEngine>();
            documentApiClientMock = mockRepository.Create<IDocumentApiClient>();
            serializationServiceMock = mockRepository.Create<ISerializationService>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            fieldMapperMock = mockRepository.Create<IFieldMapper>();

            target = new InvoiceReadyForRecognitionMessageHandler(fieldMapperMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, recognitionEngineMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, applicationContextMock.Object);
            operationName = target.GetType().Name;
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenFieldMapperIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceReadyForRecognitionMessageHandler(null, serviceBusPublisherMock.Object, telemetryClientMock.Object, recognitionEngineMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenServiceBusPublisherIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceReadyForRecognitionMessageHandler(fieldMapperMock.Object, null, telemetryClientMock.Object, recognitionEngineMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceReadyForRecognitionMessageHandler(fieldMapperMock.Object, serviceBusPublisherMock.Object, null, recognitionEngineMock.Object, documentApiClientMock.Object, serializationServiceMock.Object, applicationContextMock.Object));
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
        public async Task HandleAsync_WhenFileIdIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string fileId)
        {
            var message = CreateMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            message.FileId = fileId;
            message.TenantId = tenantId;

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task HandleAsync_WhenInvoiceIdIsZeroOrNegative_ShouldThrowArgumentException(int invoiceId)
        {
            var message = CreateMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            message.InvoiceId = invoiceId;
            message.TenantId = tenantId;

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
        public Task HandleAsync_WhenMessageIsValid_ShouldHandleMessage()
        {
            //TODO: 8979 - Implement InvoicePagesCreatedMessageHandler Tests

            return Task.CompletedTask;
        }

        private InvoiceReadyForRecognitionMessage CreateMessage()
        {
            return new InvoiceReadyForRecognitionMessage
            {
                FileId = fileId,
                InvoiceId = invoiceId,
                Fields = new List<Field>()
                //TODO: 8979 - Implement InvoicePagesCreatedMessageHandler Tests
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

        private InvoiceReadyForRecognitionMessageHandler target;
        private string operationName;

        private MockRepository mockRepository;
        private Mock<IOperation> operationMock;
        private Mock<ITelemetryClient> telemetryClientMock;
        private Mock<IServiceBusPublisher> serviceBusPublisherMock;
        private Mock<IRecognitionEngine> recognitionEngineMock;
        private Mock<IDocumentApiClient> documentApiClientMock;
        private Mock<ISerializationService> serializationServiceMock;
        private Mock<IApplicationContext> applicationContextMock;
        private Mock<IFieldMapper> fieldMapperMock;
        

        private CancellationToken cancellationToken;
        private readonly string fileId = "4e461b57-4def-4862-899f-3c144a07cc65.pdf";
        private readonly int invoiceId = 1014;
        private const string tenantId = "11";
    }
}
