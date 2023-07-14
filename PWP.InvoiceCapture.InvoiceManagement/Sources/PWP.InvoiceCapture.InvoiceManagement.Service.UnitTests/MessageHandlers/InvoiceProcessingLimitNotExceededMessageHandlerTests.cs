using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Service.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Service.UnitTests.MessageHandlers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceProcessingLimitNotExceededMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            cancellationToken = CancellationToken.None;
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceDocumentServiceMock = mockRepository.Create<IInvoiceDocumentService>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            operationMock = mockRepository.Create<IOperation>();
            invoicePageServiceMock = mockRepository.Create<IInvoicePageService>();
            fieldServiceMock = mockRepository.Create<IFieldService>();
            serviceBusPublisherMock = mockRepository.Create<IServiceBusPublisher>();
            target = new InvoiceProcessingLimitNotExceededMessageHandler(fieldServiceMock.Object, invoiceDocumentServiceMock.Object, invoicePageServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object);
            operationName = target.GetType().Name;
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsNull_ShouldThrowArgumentNullException()
        {
            SetupTelemetryClientMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.HandleAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task HandleAsync_WhenMessageFileIdNullOrEmpty_ShouldThrowArgumentNullException(string fileId)
        {
            var message = CreateInvoiceProcessingLimitNotExceededMessage(fileId, 1);
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [DataRow(0)]
        [DataRow(-1)]
        [TestMethod]
        public async Task HandleAsync_WhenMessageInvoiceIdZeroOrLessZero_ShouldThrowArgumentNullException(int invoiceId)
        {
            var message = CreateInvoiceProcessingLimitNotExceededMessage("fileId", invoiceId);
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        public void HandlerInstance_WhenFieldServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingLimitNotExceededMessageHandler(null, invoiceDocumentServiceMock.Object, invoicePageServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenInvoiceDocumentServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingLimitNotExceededMessageHandler(fieldServiceMock.Object, null, invoicePageServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenInvoicePageServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingLimitNotExceededMessageHandler(fieldServiceMock.Object, invoiceDocumentServiceMock.Object, null, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenServiceBusPublisherServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingLimitNotExceededMessageHandler(fieldServiceMock.Object, invoiceDocumentServiceMock.Object, invoicePageServiceMock.Object, null, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenTelemetryClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingLimitNotExceededMessageHandler(fieldServiceMock.Object, invoiceDocumentServiceMock.Object, invoicePageServiceMock.Object, serviceBusPublisherMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceProcessingLimitNotExceededMessageHandler(fieldServiceMock.Object, invoiceDocumentServiceMock.Object, invoicePageServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, null));
        }

        [TestMethod]
        [DataRow("PdfSample")]
        public async Task HandleAsync_WhenMessageIsValid_ShouldHandleMessage(string fileId)
        {
            const int invoiceId = 1;
            var message = CreateInvoiceProcessingLimitNotExceededMessage(fileId, invoiceId);
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            var bytes = File.ReadAllBytes($"Files/{pdfFileName}.pdf");
            var stream = new MemoryStream(bytes);

            var invoicePages = new List<InvoicePage>() { CreateInvoicePage(1, invoiceId), CreateInvoicePage(2, invoiceId) };

            fieldServiceMock
                .Setup(fieldService => fieldService.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Field>());

            invoiceDocumentServiceMock
                .Setup(invoiceDocumentService => invoiceDocumentService.GetInvoicePagesAsync(invoiceId, fileId, cancellationToken))
                .ReturnsAsync(invoicePages);

            var publishedMessages = new List<InvoiceReadyForRecognitionMessage>();
            serviceBusPublisherMock
                .Setup(serviceBusPublisher => serviceBusPublisher.PublishAsync(Capture.In(publishedMessages), cancellationToken))
                .Returns(Task.CompletedTask);

            var createdPages = new List<List<InvoicePage>>();
            invoicePageServiceMock
                .Setup(invoicePageService => invoicePageService.CreateAsync(Capture.In(createdPages), cancellationToken))
                .Returns(Task.CompletedTask);
            SetupApplicationContextSetMock();
            SetupApplicationContextGetMock();

            SetupTelemetryClientMock();

            await target.HandleAsync(brokeredMessage, cancellationToken);
            Assert.AreEqual(1,publishedMessages.Count);
            var publishedMessage = publishedMessages.First();
            Assert.AreEqual(2, publishedMessage.Pages.Count);
            Assert.AreEqual(tenantId, publishedMessage.TenantId);
            CollectionAssert.AreEquivalent(createdPages.First(), publishedMessage.Pages);
            CheckPageFields(createdPages.First());
        }

        private void CheckPageFields(List<InvoicePage> pages) 
        {
            Assert.IsTrue(pages.All(page => page.Width == width));
            Assert.IsTrue(pages.All(page => page.Height == height));

            Assert.IsTrue(pages.Any(page => page.Number == 1));
            Assert.IsTrue(pages.Any(page => page.Number == 2));

            Assert.IsTrue(pages.Any(page => page.ImageFileId == "1"));
            Assert.IsTrue(pages.Any(page => page.ImageFileId == "2"));

            Assert.IsTrue(pages.Any(page => page.InvoiceId == 1));

            Assert.AreEqual(6, pages[0].GetType().GetProperties().Length);
        }

        private InvoicePage CreateInvoicePage(int id, int invoiceId)
        {
            return new InvoicePage
            {
                Id = id,
                Height = height,
                Width = width,
                ImageFileId = id.ToString(),
                InvoiceId = invoiceId,
                Number = id
            };
        }

        private InvoiceProcessingLimitNotExceededMessage CreateInvoiceProcessingLimitNotExceededMessage(string fileId, int invoiceId)
        {
            return new InvoiceProcessingLimitNotExceededMessage
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

        private InvoiceProcessingLimitNotExceededMessageHandler target;
        private MockRepository mockRepository;
        private string operationName;
        private Mock<IOperation> operationMock;
        private Mock<ITelemetryClient> telemetryClientMock;
        private Mock<IApplicationContext> applicationContextMock;
        private Mock<IInvoiceDocumentService> invoiceDocumentServiceMock;
        private Mock<IInvoicePageService> invoicePageServiceMock;
        private Mock<IFieldService> fieldServiceMock;
        private Mock<IServiceBusPublisher> serviceBusPublisherMock;
        private CancellationToken cancellationToken;

        private const string pdfFileName = "PdfSample";
        private const string imageFileName = "0";
        private const string tenantId = "11";
        private const string cultureUs = "en-Us";
        private const int width = 860;
        private const int height = 1280;
    }
}
