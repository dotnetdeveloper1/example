using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.API.Versions.V1_0.Controllers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.API.UnitTests.Versions.V1_0.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ProcessingResultsControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceProcessingResultServiceMock = mockRepository.Create<IInvoiceProcessingResultService>();
            target = new ProcessingResultsController(invoiceProcessingResultServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoiceProcessingResultServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ProcessingResultsController(null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetProcessingResultsByInvoiceIdAsync_WhenInvoiceIdIsInvalid_ShouldReturnBadRequest(int invoiceId)
        {
            var result = await target.GetProcessingResultsByInvoiceIdAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "InvoiceId should be greater then 0.");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetProcessingResultsByIdAsync_WhenProcessingResultsIdInvalid_ShouldReturnBadRequest(int processingResultsId)
        {
            var result = await target.GetProcessingResultByIdAsync(processingResultsId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "ProcessingResultsId should be greater then 0.");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetLatestProcessingResultByInvoiceIdAsync_WhenProcessingResultsIdInvalid_ShouldReturnBadRequest(int processingResultsId)
        {
            var result = await target.GetLatestProcessingResultByInvoiceIdAsync(processingResultsId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "InvoiceId should be greater then 0.");
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetProcessingResultByIdAsync_WhenNoProcessingResult_ShouldReturnNotFound(int processingResultId)
        {
            invoiceProcessingResultServiceMock
                .Setup(invoiceProcessingResultService => invoiceProcessingResultService.GetAsync(processingResultId, cancellationToken))
                .ReturnsAsync((InvoiceProcessingResult)null);

            var result = await target.GetProcessingResultByIdAsync(processingResultId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Cannot find processing result for ProcessingResultId: {processingResultId}.");
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetLatestProcessingResultByInvoiceIdAsync_WhenNoProcessingResult_ShouldReturnNotFound(int processingResultId)
        {
            invoiceProcessingResultServiceMock
                .Setup(invoiceProcessingResultService => invoiceProcessingResultService.GetLatestAsync(processingResultId, cancellationToken))
                .ReturnsAsync((InvoiceProcessingResult)null);

            var result = await target.GetLatestProcessingResultByInvoiceIdAsync(processingResultId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Cannot find processing result for InvoiceId: {processingResultId}.");
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetProcessingResultsByInvoiceIdAsync_WhenNoProcessingResults_ShouldReturnNotFound(int invoiceId)
        {
            var emptyList = new List<InvoiceProcessingResult>();
            invoiceProcessingResultServiceMock
                .Setup(invoiceProcessingResultService => invoiceProcessingResultService
                    .GetListAsync(invoiceId, cancellationToken))
                .ReturnsAsync(emptyList);

            var result = await target.GetProcessingResultsByInvoiceIdAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Cannot find processing results for InvoiceId: {invoiceId}.");
        }

        [TestMethod]
        public async Task GetProcessingResultsByInvoiceIdAsync_WhenInvoiceIdAndProcessingResultsExist_ShouldReturnProcessingResults()
        {
            var invoiceId = processingResultInvoiceId;
            var processingResults = new List<InvoiceProcessingResult> { CreateProcessingResult(processingResultInvoiceId), CreateProcessingResult(2) };

            invoiceProcessingResultServiceMock
                .Setup(invoiceProcessingResultService => invoiceProcessingResultService.GetListAsync(invoiceId, cancellationToken))
                .ReturnsAsync(processingResults);

            var result = await target.GetProcessingResultsByInvoiceIdAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = ((ObjectResult)result).Value as ApiResponse<List<InvoiceProcessingResult>>;

            Assert.IsNotNull(apiResponce);
            Assert.IsNotNull(apiResponce.Data);

            var actualProcessingResults = apiResponce.Data;

            Assert.AreEqual(actualProcessingResults.Count, processingResults.Count);

            Assert.AreEqual(actualProcessingResults[0], processingResults[0]);
            Assert.AreEqual(actualProcessingResults[1], processingResults[1]);

            Assert.AreEqual(actualProcessingResults[0].Id, processingResults[0].Id);
            Assert.AreEqual(actualProcessingResults[1].Id, processingResults[1].Id);
        }

        [TestMethod]
        public async Task GetProcessingResultByIdAsync_WhenProcessingResultExists_ShouldReturnProcessingResult()
        {
            var processingResultId = processingResultInvoiceId;
            var expectedProcessingResult = CreateProcessingResult(processingResultInvoiceId);

            invoiceProcessingResultServiceMock
                .Setup(invoiceProcessingResultService => invoiceProcessingResultService.GetAsync(processingResultId, cancellationToken))
                .ReturnsAsync(expectedProcessingResult);

            var result = await target.GetProcessingResultByIdAsync(processingResultId, cancellationToken);

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = ((ObjectResult)result).Value as ApiResponse<InvoiceProcessingResult>;

            Assert.IsNotNull(apiResponce);
            Assert.IsNotNull(apiResponce.Data);

            var actual = apiResponce.Data;

            Assert.AreEqual(actual, expectedProcessingResult);
            Assert.AreEqual(actual.Id, expectedProcessingResult.Id);
        }

        [TestMethod]
        public async Task GetLatestProcessingResultByInvoiceIdAsync_WhenProcessingResultExists_ShouldReturnProcessingResult()
        {
            var processingResultId = processingResultInvoiceId;
            var expectedProcessingResult = CreateProcessingResult(processingResultInvoiceId);

            invoiceProcessingResultServiceMock
                .Setup(processingResultService => processingResultService.GetLatestAsync(processingResultId, cancellationToken))
                .ReturnsAsync(expectedProcessingResult);

            var result = await target.GetLatestProcessingResultByInvoiceIdAsync(processingResultId, cancellationToken);

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = ((ObjectResult)result).Value as ApiResponse<InvoiceProcessingResult>;

            Assert.IsNotNull(apiResponce);
            Assert.IsNotNull(apiResponce.Data);

            var actual = apiResponce.Data;

            Assert.AreEqual(actual, expectedProcessingResult);
            Assert.AreEqual(actual.Id, expectedProcessingResult.Id);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UpdateDataAnnotationAsync_WhenProcessingResultIdIsInvalid_ShouldReturnBadRequest(int processingResultId)
        {
            var result = await target.UpdateDataAnnotationAsync(processingResultId, null, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "ProcessingResultId should be greater then 0.");
        }

        [DataRow(1)]
        [TestMethod]
        public async Task UpdateDataAnnotationAsync_WhenProcessingResultIdIsValid_ShouldReturnObjectResult(int processingResultId)
        {
            invoiceProcessingResultServiceMock
                .Setup(processingResultService => processingResultService.UpdateDataAnnotationAsync(It.IsAny<int>(), It.IsAny<UpdatedDataAnnotation>(), cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = Core.Enumerations.OperationResultStatus.Success });
            var result = await target.UpdateDataAnnotationAsync(processingResultId, new UpdatedDataAnnotation(), cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task CompleteAsync_WhenProcessingResultIdIsInvalid_ShouldReturnBadRequest(int processingResultId)
        {
            var result = await target.CompleteAsync(processingResultId, null, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "ProcessingResultId should be greater then 0.");
        }

        [DataRow(1)]
        [TestMethod]
        public async Task CompleteAsync_WhenProcessingResultIdIsValid_ShouldReturnObjectResult(int processingResultId)
        {
            invoiceProcessingResultServiceMock
                .Setup(processingResultService => processingResultService.CompleteAsync(It.IsAny<int>(), It.IsAny<UpdatedDataAnnotation>(), cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = Core.Enumerations.OperationResultStatus.Success });

            var result = await target.CompleteAsync(processingResultId, new UpdatedDataAnnotation(), cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult));
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType, string expectedMessage)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;

            Assert.IsNotNull(objectResult.Value);
            Assert.IsInstanceOfType(objectResult.Value, typeof(ApiResponse));

            var apiResponse = (ApiResponse)objectResult.Value;
            Assert.AreEqual(expectedMessage, apiResponse.Message);
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;
            Assert.IsNotNull(objectResult.Value);
        }

        private InvoiceProcessingResult CreateProcessingResult(int id)
        {
            return new InvoiceProcessingResult()
            {
                Id = id,
                InvoiceId = processingResultInvoiceId,
                CreatedDate = DateTime.UtcNow,
                DataAnnotationFileId = "DataAnnotationFileId",
                ModifiedDate = DateTime.UtcNow,
                ProcessingType = Business.Contract.Enumerations.InvoiceProcessingType.OCR,
                TemplateId = "TemplateId",
            };
        }

        private Mock<IInvoiceProcessingResultService> invoiceProcessingResultServiceMock;
        private ProcessingResultsController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int processingResultInvoiceId = 1;
    }
}
