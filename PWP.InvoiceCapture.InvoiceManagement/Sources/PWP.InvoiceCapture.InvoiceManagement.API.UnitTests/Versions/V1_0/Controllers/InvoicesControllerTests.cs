using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.API.Versions.V1_0.Controllers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.API.UnitTests.Versions.V1_0.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoicesControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceServiceMock = mockRepository.Create<IInvoiceService>();
            target = new InvoicesController(invoiceServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoiceServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoicesController(null));
        }

        [TestMethod]
        public async Task GetInvoiceListAsync_WhenInvoicesCollectionExists_ShouldReturnOkResultWithInvoiceList()
        {
            var expectedInvoices = new List<Invoice> { new Invoice() };

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedInvoices);

            var result = await target.GetInvoiceListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<Invoice>>;

            Assert.IsInstanceOfType(apiResponce.Data, expectedInvoices.GetType());
            Assert.AreEqual(expectedInvoices, apiResponce.Data);
        }

        [TestMethod]
        public async Task GetInvoiceListAsync_WhenInvoicesNotFound_ShouldReturnOkResultWithNull()
        {
            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetListAsync(cancellationToken))
                .ReturnsAsync((List<Invoice>)null);

            var result = await target.GetInvoiceListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<Invoice>>;

            Assert.IsNull(apiResponce.Data);
        }

        [TestMethod]
        public async Task GetActiveInvoiceListAsync_WhenInvoicesCollectionExists_ShouldReturnOkResultWithInvoiceList()
        {
            var expectedInvoices = new List<Invoice> { new Invoice() };

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetActiveListAsync(cancellationToken))
                .ReturnsAsync(expectedInvoices);

            var result = await target.GetActiveInvoiceListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<Invoice>>;

            Assert.IsInstanceOfType(apiResponce.Data, expectedInvoices.GetType());
            Assert.AreEqual(expectedInvoices, apiResponce.Data);
        }

        [TestMethod]
        public async Task GetActiveInvoiceListAsync_WhenInvoicesNotFound_ShouldReturnOkResultWithNull()
        {
            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetActiveListAsync(cancellationToken))
                .ReturnsAsync((List<Invoice>)null);

            var result = await target.GetActiveInvoiceListAsync(cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<Invoice>>;

            Assert.IsNull(apiResponce.Data);
        }

        [TestMethod]
        public async Task GetPaginatedListAsync_WhenInvoicesCollectionExists_ShouldReturnOkResultWithPaginatedResult()
        {
            var paginatedRequest = new InvoicePaginatedRequest();
            var expectedInvoices = new List<Invoice> { new Invoice() };
            var expectedPaginatedResult = new PaginatedResult<Invoice>
            {
                TotalItemsCount = expectedInvoices.Count,
                Items = expectedInvoices
            };

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetPaginatedListAsync(paginatedRequest, cancellationToken))
                .ReturnsAsync(expectedPaginatedResult);

            var result = await target.GetPaginatedListAsync(paginatedRequest, cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<PaginatedResult<Invoice>>;

            Assert.IsInstanceOfType(apiResponce.Data, expectedPaginatedResult.GetType());
            Assert.AreEqual(expectedPaginatedResult, apiResponce.Data);
            Assert.AreEqual(expectedPaginatedResult.TotalItemsCount, apiResponce.Data.TotalItemsCount);
            Assert.AreEqual(expectedPaginatedResult.Items, apiResponce.Data.Items);
        }

        [TestMethod]
        public async Task GetActivePaginatedListAsync_WhenInvoicesCollectionExists_ShouldReturnOkResultWithPaginatedResult()
        {
            var paginatedRequest = new InvoicePaginatedRequest();
            var expectedInvoices = new List<Invoice> { new Invoice() };
            var expectedPaginatedResult = new PaginatedResult<Invoice>
            {
                TotalItemsCount = expectedInvoices.Count,
                Items = expectedInvoices
            };

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetActivePaginatedListAsync(paginatedRequest, cancellationToken))
                .ReturnsAsync(expectedPaginatedResult);

            var result = await target.GetActivePaginatedListAsync(paginatedRequest, cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<PaginatedResult<Invoice>>;

            Assert.IsInstanceOfType(apiResponce.Data, expectedPaginatedResult.GetType());
            Assert.AreEqual(expectedPaginatedResult, apiResponce.Data);
            Assert.AreEqual(expectedPaginatedResult.TotalItemsCount, apiResponce.Data.TotalItemsCount);
            Assert.AreEqual(expectedPaginatedResult.Items, apiResponce.Data.Items);
        }

        [TestMethod]
        [DataRow(0, 10)]
        [DataRow(10, 0)]
        [DataRow(0, 0)]
        public void InvoicePaginatedRequest_WhenInvalid_ShouldSetInvalidModelState(int pageNumber, int itemsPerPage)
        {
            var paginatedRequest = new InvoicePaginatedRequest
            {
                PageNumber = pageNumber,
                ItemsPerPage = itemsPerPage
            };

            var context = new ValidationContext(paginatedRequest, null, null);
            var results = new List<ValidationResult>();

            var isModelSateValid = Validator.TryValidateObject(paginatedRequest, context, results, true);

            Assert.IsFalse(isModelSateValid);
        }

        [TestMethod]
        public async Task GetPaginatedListAsync_WhenInvalidArguments_ShouldReturnBadRequestResult()
        {
            var paginatedRequest = new InvoicePaginatedRequest();
            target.ModelState.AddModelError("error", "error");

            var result = await target.GetPaginatedListAsync(paginatedRequest, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetActivePaginatedListAsync_WhenInvalidArguments_ShouldReturnBadRequestResult()
        {
            var paginatedRequest = new InvoicePaginatedRequest();
            target.ModelState.AddModelError("error", "error");

            var result = await target.GetActivePaginatedListAsync(paginatedRequest, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetInvoiceByIdAsync_WhenInvoiceFound_ShouldReturnOkResultWithInvoice(int invoiceId)
        {
            var expectedInvoice = new Invoice();

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetAsync(invoiceId, cancellationToken))
                .ReturnsAsync(expectedInvoice);

            var result = await target.GetInvoiceByIdAsync(invoiceId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<Invoice>;

            Assert.IsNotNull(apiResponce);
            Assert.IsInstanceOfType(apiResponce.Data, expectedInvoice.GetType());
            Assert.AreEqual(expectedInvoice, apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetInvoiceByIdAsync_WhenInvoiceNotFound_ShouldReturn404NotFoundResult(int invoiceId)
        {
            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetAsync(invoiceId, cancellationToken))
                .ReturnsAsync((Invoice)null);

            var result = await target.GetInvoiceByIdAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Invoice with id = {invoiceId} not found.");
        }

        [TestMethod]
        [DataRow(-10)]
        [DataRow(0)]
        public async Task GetInvoiceByIdAsync_WhenInvoiceIdParameterOutOfRange_ShouldReturnBadRequestResult(int invoiceId)
        {
            var result = await target.GetInvoiceByIdAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "invoiceId parameter has invalid value.");
        }

        [TestMethod]
        [DataRow(1, "test link 1")]
        [DataRow(10, "test link 1")]
        public async Task GetDocumentFileLinkAsync_WhenDocumentFileFound_ShouldReturnOkResultWithDocumentLink(int invoiceId, string expectedLink)
        {
            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetDocumentFileLinkAsync(invoiceId, cancellationToken))
                .ReturnsAsync(expectedLink);

            var result = await target.GetDocumentFileLinkAsync(invoiceId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<string>;

            Assert.IsNotNull(apiResponce);
            Assert.IsInstanceOfType(apiResponce.Data, typeof(string));
            Assert.AreEqual(expectedLink, apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task GetDocumentFileLinkAsync_WhenDocumentLinkIsNullOrEmpty_ShouldReturn404NotFoundResult(int invoiceId)
        {
            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetDocumentFileLinkAsync(invoiceId, cancellationToken))
                .ReturnsAsync((string)null);

            var result = await target.GetDocumentFileLinkAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Document file for Invoice with id = {invoiceId} not found.");
        }

        [TestMethod]
        [DataRow(-10)]
        [DataRow(0)]
        public async Task GetDocumentFileLinkAsync_WhenInvoiceIdParameterOutOfRange_ShouldReturnBadRequestResult(int invoiceId)
        {
            var result = await target.GetDocumentFileLinkAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "invoiceId parameter has invalid value.");
        }

        [TestMethod]
        [DataRow(-12)]
        [DataRow(0)]
        public async Task ArchiveInvoiceAsync_WhenInvoiceIdParameterOutOfRange_ShouldReturnBadRequestResult(int invoiceId)
        {
            var result = await target.ArchiveInvoiceAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "invoiceId parameter has invalid value.");
        }

        [DataRow(1)]
        [TestMethod]
        public async Task RedoInvoiceAsync_WhenInvoiceIdIsValid_ShouldReturnObjectResult(int invoiceId)
        {
            invoiceServiceMock
                .Setup(invoiceService => invoiceService.RedoAsync(invoiceId, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = Core.Enumerations.OperationResultStatus.Success });

            var result = await target.RedoInvoiceAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult));
        }

        [TestMethod]
        [DataRow(-12)]
        [DataRow(0)]
        public async Task RedoInvoiceAsync_WhenInvoiceIdParameterOutOfRange_ShouldReturnBadRequestResult(int invoiceId)
        {
            var result = await target.RedoInvoiceAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "invoiceId parameter has invalid value.");
        }

        [DataRow(1)]
        [TestMethod]
        public async Task ArchiveInvoiceAsync_WhenInvoiceIdIsValid_ShouldReturnObjectResult(int invoiceId)
        {
            invoiceServiceMock
                .Setup(invoiceService => invoiceService.UpdateStateAsync(invoiceId, InvoiceState.Archived, cancellationToken))
                .ReturnsAsync(new OperationResult() { Status = Core.Enumerations.OperationResultStatus.Success });

            var result = await target.ArchiveInvoiceAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(ObjectResult));
        }

        [TestMethod]
        [DataRow("fileId.pdf")]
        [DataRow("fileId.png")]
        public async Task GetInvoiceByDocumentIdAsync_WhenInvoiceFound_ShouldReturnOkResultWithInvoice(string documentId)
        {
            var expectedInvoice = new Invoice();

            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetAsync(documentId, cancellationToken))
                .ReturnsAsync(expectedInvoice);

            var result = await target.GetInvoiceByDocumentIdAsync(documentId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<Invoice>;

            Assert.IsNotNull(apiResponce);
            Assert.IsInstanceOfType(apiResponce.Data, expectedInvoice.GetType());
            Assert.AreEqual(expectedInvoice, apiResponce.Data);
        }

        [TestMethod]
        [DataRow("fileId.pdf")]
        [DataRow("fileId.png")]
        public async Task GetInvoiceByDocumentIdAsync_WhenInvoiceNotFound_ShouldReturn404NotFoundResult(string documentId)
        {
            invoiceServiceMock
                .Setup(invoiceService => invoiceService.GetAsync(documentId, cancellationToken))
                .ReturnsAsync((Invoice)null);

            var result = await target.GetInvoiceByDocumentIdAsync(documentId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Invoice with file id = {documentId} not found.");
        }

        [TestMethod]
        [DataRow("   ")]
        [DataRow("")]
        [DataRow(null)]
        public async Task GetInvoiceByDocumentIdAsync_WhenDocumentIdIsNullOrEmpty_ShouldReturnBadRequestResult(string documentId)
        {
            var result = await target.GetInvoiceByDocumentIdAsync(documentId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "documentId parameter is empty.");
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

        private Mock<IInvoiceService> invoiceServiceMock;
        private InvoicesController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
