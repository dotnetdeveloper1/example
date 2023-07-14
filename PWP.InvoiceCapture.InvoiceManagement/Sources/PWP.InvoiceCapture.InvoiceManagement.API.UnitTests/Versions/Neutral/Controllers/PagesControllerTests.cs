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
    public class PagesControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoicePagesServiceMock = mockRepository.Create<IInvoicePageService>();
            cancellationToken = CancellationToken.None;
            target = new PagesController(invoicePagesServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenInvoicePageServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PagesController(null));
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetImageLinkAsync_WhenLinkIsNull_ShouldReturnNotFound(int pageId)
        {
            invoicePagesServiceMock
                .Setup(pagesService => pagesService.GetImageLinkAsync(pageId, cancellationToken))
                .ReturnsAsync((string)null);

            var result = await target.GetImageLinkAsync(pageId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Cannot find page with PageId: {pageId}.");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetImageLinkAsync_WhenPageIdIsInvalid_ShouldReturnBadRequest(int pageId)
        {
            var result = await target.GetImageLinkAsync(pageId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "PageId should be greater then 0.");
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task GetImageLinkAsync_WhenPageExist_ShouldReturnLink(int pageId)
        {
            var page = CreateInvoicePage(pageId);
            var pageLink1 = "link1";

            invoicePagesServiceMock
                .Setup(invoicePagesService => invoicePagesService.GetImageLinkAsync(pageId, cancellationToken))
                .ReturnsAsync(pageLink1);

            var result = await target.GetImageLinkAsync(pageId, cancellationToken);

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = ((ObjectResult)result).Value as ApiResponse<string>;

            Assert.IsNotNull(apiResponce);
            Assert.IsNotNull(apiResponce.Data);

            var link = apiResponce.Data;
            Assert.AreEqual(link, pageLink1);
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetInvoicePagesAsync_WhenNoPages_ShouldReturnNotFound(int invoiceId)
        {
            var emptyList = new List<InvoicePage>();

            invoicePagesServiceMock
                .Setup(pagesService => pagesService.GetListAsync(invoiceId, cancellationToken))
                .ReturnsAsync(emptyList);

            var result = await target.GetInvoicePagesAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(NotFoundObjectResult), $"Cannot find pages for InvoiceId: {invoiceId}.");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetInvoicePagesAsync_WhenInvoiceIdIsNotValid_ShouldReturnBadRequest(int invoiceId)
        {
            var result = await target.GetInvoicePagesAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(BadRequestObjectResult), "InvoiceId should be greater then 0.");
        }

        [TestMethod]
        public async Task GetInvoicePagesAsync_WhenInvoiceIdAndPagesExist_ShouldReturnInvoicePages()
        {
            var invoiceId = 1;
            var pagesList = new List<InvoicePage> { CreateInvoicePage(1), CreateInvoicePage(2) };

            invoicePagesServiceMock
                .Setup(invoicePagesService => invoicePagesService.GetListAsync(invoiceId, cancellationToken))
                .ReturnsAsync(pagesList);

            var result = await target.GetInvoicePagesAsync(invoiceId, cancellationToken);

            AssertActionResultIsValid(result, typeof(OkObjectResult));

            var apiResponce = ((ObjectResult)result).Value as ApiResponse<List<InvoicePage>>;

            Assert.IsNotNull(apiResponce);
            Assert.IsNotNull(apiResponce.Data);

            var links = apiResponce.Data;

            Assert.AreEqual(links[0].Id, pagesList[0].Id);
            Assert.AreEqual(links[0].Height, pagesList[0].Height);
            Assert.AreEqual(links[0].ImageFileId, pagesList[0].ImageFileId);
            Assert.AreEqual(links[0].Width, pagesList[0].Width);

            Assert.AreEqual(links[1].Id, pagesList[1].Id);
            Assert.AreEqual(links[1].Height, pagesList[1].Height);
            Assert.AreEqual(links[1].ImageFileId, pagesList[1].ImageFileId);
            Assert.AreEqual(links[1].Width, pagesList[1].Width);
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;
            Assert.IsNotNull(objectResult.Value);
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

        private InvoicePage CreateInvoicePage(int id)
        {
            return new InvoicePage()
            {
                Id = id,
                ImageFileId = $"{id}.png",
                InvoiceId = 1,
                Number = id,
                Width = 620,
                Height = 1280
            };
        }

        private MockRepository mockRepository;
        private Mock<IInvoicePageService> invoicePagesServiceMock;
        private PagesController target;
        private CancellationToken cancellationToken;
    }
}
