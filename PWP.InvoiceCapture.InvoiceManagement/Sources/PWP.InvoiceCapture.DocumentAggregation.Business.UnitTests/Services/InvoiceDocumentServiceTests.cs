using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.Document.API.Client.Models;
using PWP.InvoiceCapture.DocumentAggregation.Business.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceDocumentServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            documentApiClientMock = mockRepository.Create<IDocumentApiClient>();
            serviceBusPublisherMock = mockRepository.Create<IServiceBusPublisher>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            target = new InvoiceDocumentService(documentApiClientMock.Object, serviceBusPublisherMock.Object, applicationContextMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenDocumentApiClientIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new InvoiceDocumentService(null, serviceBusPublisherMock.Object, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenServiceBusPublisherIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new InvoiceDocumentService(documentApiClientMock.Object, null, applicationContextMock.Object));
        }

        [TestMethod]
        public void Instance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoiceDocumentService(documentApiClientMock.Object, serviceBusPublisherMock.Object, null));
        }

        [TestMethod]
        public async Task SaveAsync_WhenFileContentIsNull_ShouldThrowArgumentNullException() 
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => 
                target.SaveAsync(null, fileName, apiSourceType, cancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task SaveAsync_WhenFileNameIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string fileName)
        {
            using (var fileContent = new MemoryStream())
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                    target.SaveAsync(fileContent, fileName, apiSourceType, cancellationToken));
            }
        }

        [TestMethod]
        public async Task SaveAsync_WhenValidParameters_ShouldUploadDocumentAndPublishToServiceBus() 
        {
            var uploadResponse = new UploadDocumentResponse { FileId = fileId };
            var apiResponse = new ApiResponse<UploadDocumentResponse> { Data = uploadResponse };
            var fileContent = new MemoryStream();
            var actualServiceBusMessages = new List<InvoiceDocumentUploadedMessage>();
            
            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.UploadDocumentAsync(fileContent, fileName, cancellationToken))
                .ReturnsAsync(apiResponse);

            serviceBusPublisherMock
                .Setup(publisher => publisher.PublishAsync(Capture.In(actualServiceBusMessages), cancellationToken))
                .Returns(Task.CompletedTask);

            applicationContextMock
                .Setup(applicationContext => applicationContext.TenantId)
                .Returns(tenantId);
            applicationContextMock
                .Setup(applicationContext => applicationContext.Culture)
                .Returns(cultureUs);

            var uploadedDocument = await target.SaveAsync(fileContent, fileName, apiSourceType, cancellationToken);

            var actualMessage = actualServiceBusMessages.Single();

            Assert.AreEqual(fileId, actualMessage.FileId);
            Assert.AreEqual(fileName, actualMessage.FileName);
            Assert.AreEqual(apiSourceType, actualMessage.FileSourceType);
            Assert.AreEqual(tenantId, actualMessage.TenantId);
            Assert.AreEqual(cultureUs, actualMessage.CultureName);
			Assert.IsNotNull(uploadedDocument);
	        Assert.IsNull(actualMessage.FromEmailAddress);

            // Ensure all properties are tested
            Assert.AreEqual(6, actualMessage.GetType().GetProperties().Length);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task SaveEmailDocumentAsync_WhenToIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string to)
        {
            using (var fileContent = new MemoryStream())
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                    target.SaveEmailDocumentAsync(to, emailFrom, fileName, emptyMemoryStream, cancellationToken));
            }
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task SaveEmailDocumentAsync_WhenFromIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string from)
        {
            using (var fileContent = new MemoryStream())
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                    target.SaveEmailDocumentAsync(emailTo, from, fileName, emptyMemoryStream, cancellationToken));
            }
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task SaveEmailDocumentAsync_WhenFileNameIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string fileName)
        {
            using (var fileContent = new MemoryStream())
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                    target.SaveEmailDocumentAsync(emailTo, emailFrom, fileName, emptyMemoryStream, cancellationToken));
            }
        }

        [TestMethod]
        public async Task SaveEmailDocumentAsync_WhenStreamIsNullOrWhiteSpace_ShouldThrowArgumentNullException()
        {
            using (var fileContent = new MemoryStream())
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                    target.SaveEmailDocumentAsync(emailTo, emailFrom, fileName, null, cancellationToken));
            }
        }

        [TestMethod]
        public async Task SaveEmailDocumentAsync_WhenValidParameters_ShouldUploadDocumentAndPublishToServiceBus()
        {
            var uploadResponse = new UploadDocumentResponse { FileId = fileId };
            var apiResponse = new ApiResponse<UploadDocumentResponse> { Data = uploadResponse };
            var fileStream = new MemoryStream();
            var actualServiceBusMessages = new List<EmailDocumentUploadedMessage>();

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.UploadDocumentAsync(fileStream, fileName, cancellationToken))
                .ReturnsAsync(apiResponse);

            serviceBusPublisherMock
                .Setup(publisher => publisher.PublishAsync(Capture.In(actualServiceBusMessages), cancellationToken))
                .Returns(Task.CompletedTask);

            var uploadedDocument = await target.SaveEmailDocumentAsync(emailTo, emailFrom, fileName, fileStream, cancellationToken);

            var actualMessage = actualServiceBusMessages.Single();

            Assert.AreEqual(fileId, actualMessage.FileId);
            Assert.AreEqual(fileName, actualMessage.FileName);
            Assert.AreEqual(emailSourceType, actualMessage.FileSourceType);
            Assert.AreEqual(emailTo, actualMessage.To);
            Assert.AreEqual(emailFrom, actualMessage.From);
            Assert.IsNotNull(uploadedDocument);

            // Ensure all properties are tested
            Assert.AreEqual(6, actualMessage.GetType().GetProperties().Length);
        }

        private MockRepository mockRepository;
        private Mock<IDocumentApiClient> documentApiClientMock;
        private Mock<IServiceBusPublisher> serviceBusPublisherMock;
        private Mock<IApplicationContext> applicationContextMock;
        private InvoiceDocumentService target;
        private readonly string fileId = "4e461b57-4def-4862-899f-3c144a07cc65.pdf";
        private readonly string fileName = "invoice.pdf";
        private readonly FileSourceType apiSourceType = FileSourceType.API;
        private readonly FileSourceType emailSourceType = FileSourceType.Email;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private readonly MemoryStream emptyMemoryStream = new MemoryStream();
        private const string tenantId = "11";
        private const string emailTo = "emailTo1@mail.com";
        private const string emailFrom = "emailFrom1@mail.com";
        private const string cultureUs = "en-Us";

    }
}
