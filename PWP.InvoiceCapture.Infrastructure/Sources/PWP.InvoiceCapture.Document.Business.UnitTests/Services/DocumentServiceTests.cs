using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Document.Business.Contract.Models;
using PWP.InvoiceCapture.Document.Business.Contract.Repositories;
using PWP.InvoiceCapture.Document.Business.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Document.Business.UnitTests.Services
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DocumentServiceTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            documentRepositoryMock = mockRepository.Create<IDocumentRepository>();
            target = new DocumentService(documentRepositoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenDocumentRepositoryIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DocumentService(null));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetDocumentStreamAsync_WhenFileIdIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string fileId) 
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.GetDocumentStreamAsync(fileId, cancellationToken));
        }

        [TestMethod]
        [DataRow("fileId1")]
        [DataRow("fileId2")]
        public async Task GetDocumentStreamAsync_WhenFileExists_ShouldReturnDocumentStreamResult(string fileId)
        {
            var expectedResult = new GetDocumentStreamResult
            {
                FileStream = new MemoryStream(),
                Length = 35,
                ContentType = "text/plain"
            };

            documentRepositoryMock
                .Setup(documentRepository => documentRepository.GetStreamAsync(fileId, cancellationToken))
                .ReturnsAsync(expectedResult);

            var actualOperationResult = await target.GetDocumentStreamAsync(fileId, cancellationToken);

            Assert.IsNotNull(actualOperationResult);
            Assert.IsTrue(actualOperationResult.IsSuccessful);

            var actualResult = actualOperationResult.Data;

            Assert.AreEqual(expectedResult.ContentType, actualResult.ContentType);
            Assert.AreEqual(expectedResult.Length, actualResult.Length);
            Assert.AreEqual(expectedResult.FileStream, actualResult.FileStream);
        }

        [TestMethod]
        [DataRow("fileId1")]
        [DataRow("fileId2")]
        public async Task GetDocumentStreamAsync_WhenFileNotFount_ShouldReturnNotFoundResult(string fileId)
        {
            documentRepositoryMock
                .Setup(documentRepository => documentRepository.GetStreamAsync(fileId, cancellationToken))
                .ReturnsAsync((GetDocumentStreamResult)null);

            var actualOperationResult = await target.GetDocumentStreamAsync(fileId, cancellationToken);

            Assert.IsNotNull(actualOperationResult);
            Assert.AreEqual(OperationResultStatus.NotFound, actualOperationResult.Status);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetTemporaryLinkAsync_WhenFileIdIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string fileId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.GetTemporaryLinkAsync(fileId, cancellationToken));
        }

        [TestMethod]
        [DataRow("fileId1")]
        [DataRow("fileId2")]
        public async Task GetTemporaryLinkAsync_WhenFileExists_ShouldReturnTemporaryLink(string fileId)
        {
            var expectedResult = "somelink";
            documentRepositoryMock
                .Setup(documentRepository => documentRepository.GetTemporaryLinkAsync(fileId, cancellationToken))
                .ReturnsAsync(expectedResult);

            var actualOperationResult = await target.GetTemporaryLinkAsync(fileId, cancellationToken);

            Assert.IsNotNull(actualOperationResult);

            var actualResult = actualOperationResult.Data;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        [DataRow("fileId1")]
        [DataRow("fileId2")]
        public async Task GetTemporaryLinkAsync_WhenFileNotFount_ShouldReturnNotFoundResult(string fileId)
        {
            documentRepositoryMock
                .Setup(documentRepository => documentRepository.GetTemporaryLinkAsync(fileId, cancellationToken))
                .ReturnsAsync((string)null);

            var actualOperationResult = await target.GetTemporaryLinkAsync(fileId, cancellationToken);

            Assert.IsNotNull(actualOperationResult);
            Assert.AreEqual(OperationResultStatus.NotFound, actualOperationResult.Status);
        }

        [TestMethod]
        public void CreateDocumentAsync_WhenNullStream_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateDocumentAsync(null, ".pdf", cancellationToken));
        }

        [TestMethod]
        [DataRow("This is a fake unit test file.", ".pdf")]
        [DataRow("This is a fake unit test file.", null)]
        [DataRow("This is a fake unit test file.", " ")]
        [DataRow("This is a fake unit test file.", "")]
        public async Task CreateDocumentAsync_WhenContentAndFileExtensionNotNull_ShouldReturnFileId(string content, string fileExtension)
        {
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(ms))
            {
                writer.Write(content);
                writer.Flush();
                ms.Position = 0;

                var fileIdWithoutExtension = Guid.NewGuid().ToString();
                var fileId = $"{fileIdWithoutExtension}{fileExtension}";

                documentRepositoryMock
                    .Setup(documentRepository => documentRepository.SaveAsync(It.IsAny<CreateDocumentArgs>(), cancellationToken))
                    .ReturnsAsync(fileId);

                var actualOperationResult = await target.CreateDocumentAsync(ms, fileExtension, cancellationToken);

                Assert.AreEqual(fileId, actualOperationResult);
            }
        }

        private MockRepository mockRepository;
        private DocumentService target;
        private Mock<IDocumentRepository> documentRepositoryMock;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
