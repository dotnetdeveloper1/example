using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.Identity.Service.MessageHandlers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
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
    public class EmailDocumentUploadedMessageHandlerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            cancellationToken = CancellationToken.None;
            mockRepository = new MockRepository(MockBehavior.Strict);
            tenantServiceMock = mockRepository.Create<ITenantService>();
            tenantSettingServiceMock = mockRepository.Create<ITenantSettingService>();
            cultureServiceMock = mockRepository.Create<ICultureService>();
            serviceBusPublisherMock = mockRepository.Create<IServiceBusPublisher>();
            operationMock = mockRepository.Create<IOperation>();
            telemetryClientMock = mockRepository.Create<ITelemetryClient>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            target = new EmailDocumentUploadedMessageHandler(tenantServiceMock.Object, cultureServiceMock.Object, tenantSettingServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object);
            operationName = target.GetType().Name;
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void HandlerInstance_WhenTenantServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new EmailDocumentUploadedMessageHandler(null, cultureServiceMock.Object, tenantSettingServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenServiceBusPublisherMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new EmailDocumentUploadedMessageHandler(tenantServiceMock.Object, cultureServiceMock.Object, tenantSettingServiceMock.Object, null, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenTelemetryClientMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new EmailDocumentUploadedMessageHandler(tenantServiceMock.Object, cultureServiceMock.Object, tenantSettingServiceMock.Object, serviceBusPublisherMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenApplicationContextMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new EmailDocumentUploadedMessageHandler(tenantServiceMock.Object, cultureServiceMock.Object, tenantSettingServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, null));
        }

        [TestMethod]
        public void HandlerInstance_WhenTenantSettingServiceMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new EmailDocumentUploadedMessageHandler(tenantServiceMock.Object, cultureServiceMock.Object, null, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void HandlerInstance_WhenCultureServiceMockIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new EmailDocumentUploadedMessageHandler(tenantServiceMock.Object, null, tenantSettingServiceMock.Object, serviceBusPublisherMock.Object, telemetryClientMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public async Task HandleAsync_WhenMessageIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            SetupTelemetryClientMock();
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.HandleAsync(null, cancellationToken));
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        [TestMethod]
        public async Task HandleAsync_WhenMessageEmailIsNullOrWhitespace_ShouldThrowArgumentExceptionAsync(string email)
        {
            var message = CreateEmailDocumentUploadedMessage(email);
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.HandleAsync(brokeredMessage, cancellationToken));
        }

        [TestMethod]
        public async Task HandleAsync_WhenTenantWithProvidedEmailExists_ShouldPublishMessageAsync()
        {
            var message = CreateEmailDocumentUploadedMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            tenantServiceMock
                .Setup(documentUploadedEmailService => documentUploadedEmailService.GetTenantIdByEmailAsync(to, cancellationToken))
                .ReturnsAsync(tenantId);
            
            tenantSettingServiceMock
                .Setup(tenantSettingService => tenantSettingService.GetAsync(Convert.ToInt32(tenantId), cancellationToken))
                .ReturnsAsync(new Business.Contract.Models.TenantSetting() { CultureId = cultureId});

            cultureServiceMock
                .Setup(cultureService => cultureService.GetAsync(cultureId, cancellationToken))
                .ReturnsAsync(new Business.Contract.Models.Culture() { Name = cultureUs });

            var publishedMessages = new List<TenantEmailResolvedMessage>();
           
            serviceBusPublisherMock
                .Setup(serviceBusPublisher => serviceBusPublisher.PublishAsync(Capture.In(publishedMessages), cancellationToken))
                .Returns(Task.CompletedTask);

            await target.HandleAsync(brokeredMessage, cancellationToken);

            Assert.AreEqual(publishedMessages.Count, 1);

            var publishedMessage = publishedMessages.FirstOrDefault();
           
            Assert.IsNotNull(publishedMessage);
            Assert.AreEqual(tenantId.ToString(), publishedMessage.TenantId);
            Assert.AreEqual(to, publishedMessage.To);
            Assert.AreEqual(from, publishedMessage.From);
            Assert.AreEqual(documentId, publishedMessage.FileId);
            Assert.AreEqual(documentName, publishedMessage.FileName);
            Assert.AreEqual(sourceType, publishedMessage.FileSourceType);
            Assert.AreEqual(cultureUs, publishedMessage.CultureName);
        }

        [TestMethod]
        public async Task HandleAsync_WhenTenantWithProvidedEmailNotExists_ShouldNotPublishMessageAsync()
        {
            var message = CreateEmailDocumentUploadedMessage();
            var brokeredMessage = new BrokeredMessage { InnerMessage = message };

            SetupTelemetryClientMock();
            SetupApplicationContextSetMock();

            tenantServiceMock
                .Setup(documentUploadedEmailService => documentUploadedEmailService.GetTenantIdByEmailAsync(to, cancellationToken))
                .ReturnsAsync((string)null);

            await target.HandleAsync(brokeredMessage, cancellationToken);
        }

        private EmailDocumentUploadedMessage CreateEmailDocumentUploadedMessage(string to = to)
        {
            return new EmailDocumentUploadedMessage
            {
                To = to,
                From = from,
                FileId = documentId,
                FileName = documentName,
                FileSourceType = sourceType,
            };
        }

        private void SetupApplicationContextSetMock()
        {
            applicationContextMock
                .SetupSet(applicationContext => applicationContext.TenantId = null)
                .Verifiable();
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

        private EmailDocumentUploadedMessageHandler target;
        private MockRepository mockRepository;
        private Mock<ITelemetryClient> telemetryClientMock;
        private Mock<IApplicationContext> applicationContextMock;
        private Mock<ITenantSettingService> tenantSettingServiceMock;
        private Mock<ICultureService> cultureServiceMock;
        private Mock<IServiceBusPublisher> serviceBusPublisherMock;
        private Mock<IOperation> operationMock;
        private Mock<ITenantService> tenantServiceMock;
        private CancellationToken cancellationToken;
        private string operationName;

        private const string tenantId = "11";
        private const int cultureId = 22;
        private const string cultureUs = "en-Us";
        private const string to = "someTo";
        private const string from = "someFrom";
        private const string documentId = "someId";
        private const string documentName = "someName";
        private const FileSourceType sourceType = FileSourceType.Email;
    }
}
