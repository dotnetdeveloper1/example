using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.API.Versions.Neutral.Controllers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.API.UnitTests.Versions.Neutral.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceFieldsControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoiceFieldServiceMock = mockRepository.Create<IInvoiceFieldService>();
            target = new InvoiceFieldsController(invoiceFieldServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoiceFieldServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceFieldsController(null));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetInvoiceFieldListAsync_WhenInvoiceFieldsCollectionExists_ShouldReturnOkResultWithCollection(int invoiceId)
        {
            var expectedInvoiceFields = new List<InvoiceField> { new InvoiceField() };

            invoiceFieldServiceMock
                .Setup(invoiceFieldService => invoiceFieldService.GetListAsync(invoiceId, cancellationToken))
                .ReturnsAsync(expectedInvoiceFields);

            var result = await target.GetInvoiceFieldListAsync(invoiceId, cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<InvoiceField>>;

            Assert.IsInstanceOfType(apiResponce.Data, expectedInvoiceFields.GetType());
            Assert.AreEqual(expectedInvoiceFields, apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetInvoiceFieldListAsync_WhenInvoiceFieldsNotFound_ShouldReturnOkResultWithNull(int invoiceId)
        {
            invoiceFieldServiceMock
                .Setup(invoiceFieldService => invoiceFieldService.GetListAsync(invoiceId, cancellationToken))
                .ReturnsAsync((List<InvoiceField>)null);

            var result = await target.GetInvoiceFieldListAsync(invoiceId, cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<InvoiceField>>;

            Assert.IsNull(apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetInvoiceFieldListAsync_WhenInvoiceFieldsWereFound_ShouldReturnOkResult(int invoiceId)
        {
            var expectedInvoiceFields = new List<InvoiceField>() { new InvoiceField() };

            invoiceFieldServiceMock
                .Setup(invoiceFieldService => invoiceFieldService.GetListAsync(invoiceId, cancellationToken))
                .ReturnsAsync(expectedInvoiceFields);

            var result = await target.GetInvoiceFieldListAsync(invoiceId, cancellationToken) as OkObjectResult;

            Assert.IsNotNull(result);
            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<List<InvoiceField>>;

            Assert.IsNotNull(apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetInvoiceFieldByIdAsync_WhenInvoiceFieldFound_ShouldReturnOkResultWithInvoiceField(int invoiceFieldId)
        {
            var expectedInvoiceField = new InvoiceField();

            invoiceFieldServiceMock
                .Setup(invoiceFieldService => invoiceFieldService.GetAsync(invoiceFieldId, cancellationToken))
                .ReturnsAsync(expectedInvoiceField);

            var result = await target.GetInvoiceFieldByIdAsync(invoiceFieldId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = result.Value as ApiResponse<InvoiceField>;

            Assert.IsNotNull(apiResponce);
            Assert.IsInstanceOfType(apiResponce.Data, expectedInvoiceField.GetType());
            Assert.AreEqual(expectedInvoiceField, apiResponce.Data);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetInvoiceFieldByIdAsync_WhenInvoiceFieldNotFound_ShouldReturn404NotFoundResult(int invoiceFieldId)
        {
            invoiceFieldServiceMock
                .Setup(invoiceFieldService => invoiceFieldService.GetAsync(invoiceFieldId, cancellationToken))
                .ReturnsAsync((InvoiceField)null);

            var result = await target.GetInvoiceFieldByIdAsync(invoiceFieldId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"InvoiceField with id = {invoiceFieldId} not found.");
        }

        [TestMethod]
        [DataRow(-10)]
        [DataRow(0)]
        public async Task GetInvoiceFieldByIdAsync_WhenInvoiceFieldIdParameterOutOfRange_ShouldReturnBadRequestResult(int invoiceFieldId)
        {
            var result = await target.GetInvoiceFieldByIdAsync(invoiceFieldId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), $"{nameof(invoiceFieldId)} parameter has invalid value.");
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

        private Mock<IInvoiceFieldService> invoiceFieldServiceMock;
        private InvoiceFieldsController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
