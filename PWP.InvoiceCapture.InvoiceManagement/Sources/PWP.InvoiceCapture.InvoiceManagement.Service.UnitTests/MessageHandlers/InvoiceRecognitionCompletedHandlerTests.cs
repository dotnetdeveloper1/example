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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service.UnitTests.MessageHandlers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceRecognitionCompletedMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceProcessingResultServiceMock = mockRepository.Create<IInvoiceProcessingResultService>();
            invoiceServiceMock = mockRepository.Create<IInvoiceService>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            operationMock = mockRepository.Create<IOperation>();
            innerMessage = CreateMessage();
            brokeredMessage = new BrokeredMessage { InnerMessage = innerMessage };
            target = new InvoiceRecognitionCompletedHandler(invoiceProcessingResultServiceMock.Object, invoiceServiceMock.Object, telemetryClientMock.Object, applicationContextMock.Object);
            operationName = target.GetType().Name;
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoiceProcessingResultServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceRecognitionCompletedHandler(null, invoiceServiceMock.Object, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceRecognitionCompletedHandler(invoiceProcessingResultServiceMock.Object, invoiceServiceMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceRecognitionCompletedHandler(invoiceProcessingResultServiceMock.Object, invoiceServiceMock.Object, telemetryClientMock.Object, null));
        }

        [TestMethod]
        public void Instance_WhenInvoiceServiceMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceRecognitionCompletedHandler(invoiceProcessingResultServiceMock.Object, null, telemetryClientMock.Object, applicationContextMock.Object));
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
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task HandleAsync_WhenDataAnnotationFileIdIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string dataAnnotationFileId)
        {
            innerMessage.DataAnnotationFileId = dataAnnotationFileId;
            innerMessage.TenantId = tenantId;

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();


            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task HandleAsync_WhenTemplateIdIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string templateId)
        {
            innerMessage.TemplateId = templateId;
            innerMessage.TenantId = tenantId;

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsValid_ShouldHandleMessage()
        {
            var actualProcessingResults = new List<InvoiceProcessingResult>();
            innerMessage.TenantId = tenantId;
            invoiceProcessingResultServiceMock
                .Setup(invoiceProcessingResultService => invoiceProcessingResultService.CreateAsync(Capture.In(actualProcessingResults), cancellationToken))
                .Returns(Task.CompletedTask);
            invoiceProcessingResultServiceMock
              .Setup(invoiceProcessingResultService => invoiceProcessingResultService.ValidateCreatedInvoiceAsync(innerMessage.InvoiceId, innerMessage.CultureName, cancellationToken))
              .Returns(Task.CompletedTask);
            invoiceServiceMock
              .Setup(invoiceServiceMock => invoiceServiceMock.PublishInvoiceStatusChangedMessageAsync(innerMessage.InvoiceId, innerMessage.TenantId, cancellationToken))
              .Returns(Task.CompletedTask);

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await target.HandleAsync(brokeredMessage, cancellationToken);

            Assert.AreEqual(1, actualProcessingResults.Count);

            var actualProcessingResult = actualProcessingResults.Single();

            Assert.IsNotNull(actualProcessingResult);
            Assert.AreEqual(0, actualProcessingResult.Id);
            Assert.AreEqual(innerMessage.InvoiceId, actualProcessingResult.InvoiceId);
            Assert.AreEqual(innerMessage.TemplateId, actualProcessingResult.TemplateId);
            Assert.AreEqual(innerMessage.DataAnnotationFileId, actualProcessingResult.DataAnnotationFileId);
            Assert.AreEqual(innerMessage.InvoiceProcessingType, actualProcessingResult.ProcessingType);
            Assert.AreEqual(innerMessage.TrainingFileCount, actualProcessingResult.TrainingFileCount);
            Assert.AreEqual(default, actualProcessingResult.CreatedDate);
            Assert.AreEqual(default, actualProcessingResult.ModifiedDate);
            Assert.IsNull(actualProcessingResult.CultureName);
            Assert.IsNull(actualProcessingResult.VendorName);

            // Ensure all properties are tested
            Assert.AreEqual(13, actualProcessingResult.GetType().GetProperties().Length);
        }

        private InvoiceRecognitionCompletedMessage CreateMessage()
        {
            return new InvoiceRecognitionCompletedMessage
            {
                InvoiceId = 1,
                DataAnnotationFileId = "4e461b57-4def-4862-899f-3c144a07cc65.pdf",
                TemplateId = "4587125",
                InvoiceProcessingType = InvoiceProcessingType.OCR,
                TrainingFileCount = 12
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
        private Mock<IInvoiceProcessingResultService> invoiceProcessingResultServiceMock;
        private Mock<IInvoiceService> invoiceServiceMock;
        private BrokeredMessage brokeredMessage;
        private InvoiceRecognitionCompletedMessage innerMessage;
        private InvoiceRecognitionCompletedHandler target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const string tenantId = "11";
    }
}
