using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Document.API.Client.Models;
using PWP.InvoiceCapture.Document.API.Controllers;
using PWP.InvoiceCapture.Document.Business.Contract.Models;
using PWP.InvoiceCapture.Document.Business.Contract.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Document.API.UnitTests.Controllers
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DocumentsControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            documentServiceMock = mockRepository.Create<IDocumentService>();
            fileMock = mockRepository.Create<IFormFile>();
            target = CreateTargetController();
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task CreateFileAsync_WhenFileIsNull_ShouldReturnBadRequest()
        {
            var result = await target.CreateFileAsync(null, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var objectResult = (ObjectResult)result;

            Assert.IsNotNull(objectResult.Value);
            Assert.IsInstanceOfType(objectResult.Value, typeof(ApiResponse));

            var apiResponse = (ApiResponse)objectResult.Value;
            Assert.AreEqual("File is null.", apiResponse.Message);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("fileName")]
        [DataRow("fileName.txt")]
        public async Task CreateFileAsync_WhenFileIsNotNull_ShouldSaveAndReturnFileName(string fileName)
        {
            using (var fileStream = new MemoryStream())
            {
                fileMock.Setup(file => file.OpenReadStream()).Returns(fileStream);
                fileMock.Setup(file => file.FileName).Returns(fileName);

                documentServiceMock
                    .Setup(documentService => documentService.CreateDocumentAsync(fileStream, Path.GetExtension(fileName), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedFileId);

                var result = await target.CreateFileAsync(fileMock.Object, cancellationToken);

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ObjectResult));

                var objectResult = (ObjectResult)result;

                Assert.IsNotNull(objectResult.Value);
                Assert.IsInstanceOfType(objectResult.Value, typeof(ApiResponse<UploadDocumentResponse>));

                var apiResponse = (ApiResponse<UploadDocumentResponse>)objectResult.Value;

                Assert.IsNotNull(apiResponse.Data);
                Assert.AreEqual(expectedFileId, apiResponse.Data.FileId);
            }
        }

        [TestMethod]
        [DataRow(OperationResultStatus.NotFound, typeof(NotFoundObjectResult))]
        [DataRow(OperationResultStatus.Failed, typeof(BadRequestObjectResult))]
        public async Task GetStreamAsync_WhenNotSuccessfulResult_ShouldReturnAppropriateActionResult(OperationResultStatus resultStatus, Type expectedType)
        {
            var operationResult = new OperationResult<GetDocumentStreamResult>
            {
                Status = resultStatus,
                Message = operationResultMessage,
                Code = operationResultCode
            };

            documentServiceMock
                .Setup(documentService => documentService.GetDocumentStreamAsync(expectedFileId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationResult);

            var result = await target.GetStreamAsync(expectedFileId, cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;

            Assert.IsNotNull(objectResult.Value);
            Assert.IsInstanceOfType(objectResult.Value, typeof(ApiResponse));

            var apiResponse = (ApiResponse)objectResult.Value;

            Assert.AreEqual(operationResult.Code, apiResponse.Code);
            Assert.AreEqual(operationResult.Message, apiResponse.Message);
        }

        [TestMethod]
        public async Task GetStreamAsync_WhenSuccessfulResult_ShouldReturnFileStreamResult()
        {
            using (var expectedStream = new MemoryStream())
            {
                var getDocumentStreamResult = new GetDocumentStreamResult
                {
                    FileStream = expectedStream,
                    ContentType = "text/plain",
                    Length = 123
                };

                var operationResult = OperationResult<GetDocumentStreamResult>.Success(getDocumentStreamResult);

                operationResult.Message = operationResultMessage;
                operationResult.Code = operationResultCode;

                documentServiceMock
                    .Setup(documentService => documentService.GetDocumentStreamAsync(expectedFileId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(operationResult);

                var result = await target.GetStreamAsync(expectedFileId, cancellationToken);

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(FileStreamResult));

                var fileStreamResult = (FileStreamResult)result;

                Assert.IsNotNull(fileStreamResult.FileStream);
                Assert.AreEqual(expectedStream, fileStreamResult.FileStream);
                Assert.AreEqual(getDocumentStreamResult.ContentType, fileStreamResult.ContentType);
                Assert.AreEqual(getDocumentStreamResult.Length, target.Response.ContentLength);
                Assert.AreEqual(expectedFileId, fileStreamResult.FileDownloadName);
            }
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        [DataRow("text")]
        [DataRow("invalidContentType")]
        public async Task GetStreamAsync_WhenInvalidContentType_ShouldThrowFormatException(string contentType)
        {
            using (var expectedStream = new MemoryStream())
            {
                var getDocumentStreamResult = new GetDocumentStreamResult
                {
                    FileStream = expectedStream,
                    ContentType = contentType,
                };

                var operationResult = OperationResult<GetDocumentStreamResult>.Success(getDocumentStreamResult);

                documentServiceMock
                    .Setup(documentService => documentService.GetDocumentStreamAsync(expectedFileId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(operationResult);

                await Assert.ThrowsExceptionAsync<FormatException>(() => target.GetStreamAsync(expectedFileId, cancellationToken));
            }
        }

        [TestMethod]
        public async Task GetTemporaryLinkAsync_WhenSuccessfulResult_ShouldReturnLink()
        {
            var operationResult = new OperationResult<string>
            {
                Status = OperationResultStatus.Success,
                Message = operationResultMessage,
                Code = operationResultCode
            };

            operationResult.Data = "someTestLink";
            documentServiceMock
                .Setup(documentService => documentService.GetTemporaryLinkAsync(expectedFileId, cancellationToken))
                .ReturnsAsync(operationResult);

            var result = await target.GetTemporaryLinkAsync(expectedFileId, cancellationToken);
           
            Assert.IsNotNull(result);

            var objectResult = (ObjectResult)result;

            Assert.IsInstanceOfType(objectResult.Value, typeof(ApiResponse<string>));

            var apiResponse = (ApiResponse<string>)objectResult.Value;

            Assert.AreEqual(operationResult.Code, apiResponse.Code);
            Assert.AreEqual(operationResult.Message, apiResponse.Message);
            Assert.AreEqual(operationResult.Data, apiResponse.Data);
        }


        private DocumentsController CreateTargetController()
        {
            var httpContext = new DefaultHttpContext();

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            return new DocumentsController(documentServiceMock.Object)
            {
                ControllerContext = controllerContext
            };
        }

        private MockRepository mockRepository;
        private Mock<IDocumentService> documentServiceMock;
        private Mock<IFormFile> fileMock;
        private DocumentsController target;
        private const string expectedFileId = "6e297417-1be8-4a3a-8c28-7f3778929446.txt";
        private const int operationResultCode = 123;
        private const string operationResultMessage = "Message";
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
