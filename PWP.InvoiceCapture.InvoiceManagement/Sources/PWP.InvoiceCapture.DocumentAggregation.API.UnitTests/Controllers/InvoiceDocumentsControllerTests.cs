using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.DocumentAggregation.API.Controllers;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Models;
using PWP.InvoiceCapture.DocumentAggregation.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.API.UnitTests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceDocumentsControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceDocumentServiceMock = mockRepository.Create<IInvoiceDocumentService>();
            fileMock = mockRepository.Create<IFormFile>();
            target = new InvoiceDocumentsController(invoiceDocumentServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoiceDocumentServiceIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceDocumentsController(null));
        }

        [TestMethod]
        public async Task UploadDocumentAsync_WhenFileIsNull_ShouldReturnBadRequest()
        {
            var result = await target.UploadDocumentAsync(null, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "File is null.");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task UploadDocumentAsync_WhenFileNameIsNullOrWhiteSpace_ShouldReturnBadRequest(string fileName)
        {
            fileMock
                .Setup(file => file.FileName)
                .Returns(fileName);

            var result = await target.UploadDocumentAsync(fileMock.Object, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "FileName is null or empty.");
        }

        [TestMethod]
        [DataRow("invoice.txt")]
        [DataRow("invoice.gif")]
        [DataRow("invoice")]
        [DataRow("pdf")]
        public async Task UploadDocumentAsync_WhenUnsupportedOrMissingExtension_ShouldReturnBadRequest(string fileName)
        {
            fileMock
                .Setup(file => file.FileName)
                .Returns(fileName);

            var result = await target.UploadDocumentAsync(fileMock.Object, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "File format is not supported.");
        }

        [TestMethod]
        [DataRow("invoice.png")]
        [DataRow("invoice.pdf")]
        [DataRow("invoice.bmp")]
        [DataRow("invoice.tiff")]
        [DataRow("invoice.tif")]
        [DataRow("invoice.jpeg")]
        [DataRow("invoice.jpg")]
        public async Task UploadDocumentAsync_WhenFileExtensionIsValid_ShouldSave(string fileName)
        {
            using (var fileStream = new MemoryStream())
            {
                fileMock
                    .Setup(file => file.OpenReadStream())
                    .Returns(fileStream);

                fileMock
                    .Setup(file => file.FileName)
                    .Returns(fileName);

                var uploadedDocument = CreateUploadedDocument();

                invoiceDocumentServiceMock
                    .Setup(documentService => documentService.SaveAsync(fileStream, fileName, sourceType, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(uploadedDocument);

                var result = await target.UploadDocumentAsync(fileMock.Object, cancellationToken);

                AssertActionResultIsValid(result, typeof(ObjectResult), $"{fileName} successfully uploaded.");
            }
        }

        [TestMethod]
        [DataRow("invoice.PNG")]
        [DataRow("invoice.PDF")]
        [DataRow("invoice.PnG")]
        [DataRow("invoice.PdF")]
        [DataRow("invoice.Tiff")]
        [DataRow("invoice.TIf")]
        [DataRow("invoice.jPeg")]
        [DataRow("invoice.JpG")]
        public async Task UploadDocumentAsync_WhenFileExtensionLetterCaseIsDifferent_ShouldSave(string fileName)
        {
            using (var fileStream = new MemoryStream())
            {
                fileMock
                    .Setup(file => file.OpenReadStream())
                    .Returns(fileStream);

                fileMock
                    .Setup(file => file.FileName)
                    .Returns(fileName);

                var uploadedDocument = CreateUploadedDocument();

                invoiceDocumentServiceMock
                    .Setup(documentService => documentService.SaveAsync(fileStream, fileName, sourceType, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(uploadedDocument);

                var result = await target.UploadDocumentAsync(fileMock.Object, cancellationToken);

                AssertActionResultIsValid(result, typeof(ObjectResult), $"{fileName} successfully uploaded.");
            }
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType, string expectedMessage) 
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;

            Assert.IsNotNull(objectResult.Value);
            Assert.IsInstanceOfType(objectResult.Value, typeof(ApiResponse));

            if (objectResult.StatusCode == StatusCodes.Status200OK)
            {
                var uploadDocumentApiResponce = (ApiResponse<UploadedDocument>)objectResult.Value;
                Assert.IsNotNull(uploadDocumentApiResponce.Data);
                Assert.IsFalse(string.IsNullOrWhiteSpace(uploadDocumentApiResponce.Data.DocumentId));
            }

            var apiResponse = (ApiResponse)objectResult.Value;
            Assert.AreEqual(expectedMessage, apiResponse.Message);
        }

        private UploadedDocument CreateUploadedDocument()
        {
            return new UploadedDocument { DocumentId = Guid.NewGuid().ToString() };
        }

        private Mock<IInvoiceDocumentService> invoiceDocumentServiceMock;
        private Mock<IFormFile> fileMock;
        private InvoiceDocumentsController target;
        private MockRepository mockRepository;
        private readonly FileSourceType sourceType = FileSourceType.API;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
