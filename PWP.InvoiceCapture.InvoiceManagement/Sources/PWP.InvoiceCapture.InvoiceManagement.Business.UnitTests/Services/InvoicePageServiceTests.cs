using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoicePageServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            invoicePageRepositoryMock = mockRepository.Create<IInvoicePageRepository>();
            documentApiClientMock = mockRepository.Create<IDocumentApiClient>();
            cancellationToken = CancellationToken.None;
            target = new InvoicePageService(invoicePageRepositoryMock.Object, documentApiClientMock.Object);
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
                new InvoicePageService(invoicePageRepositoryMock.Object, null));
        }

        [TestMethod]
        public void Instance_WhenInvoicePageRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvoicePageService(null, documentApiClientMock.Object));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetImageLinkAsync_WhenInvoiceIdIsWrong_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetImageLinkAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetImageLinkAsync_WhenPageIdIsWrong_ShouldThrowArgumentException(int pageId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetImageLinkAsync(pageId, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetListAsync_WhenPageIdIsWrong_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetListAsync(invoiceId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        public async Task GetImageLinkAsync_WhenInvoicePageIdIsCorrect_ShouldReturnInvoicePage(int invoicePageId)
        {
            var invoicePage = CreateInvoicePage(invoicePageId);

            invoicePageRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(invoicePageId, cancellationToken))
                .ReturnsAsync(invoicePage);

            var link = "link1";
            var documentApiResponce = new ApiResponse<string> { Data = link };

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.GetTemporaryLinkAsync(invoicePage.ImageFileId, cancellationToken))
                .ReturnsAsync(documentApiResponce);

            var actualInvoicePage = await target.GetImageLinkAsync(invoicePageId, cancellationToken);

            Assert.AreEqual(link, actualInvoicePage);
        }

        [TestMethod]
        [DataRow(1)]
        public async Task GetImageLinkAsync_WhenInvoiceIdIsIncorrect_ShouldReturnNull(int invoicePageId)
        {
            invoicePageRepositoryMock
                .Setup(invoiceRepository => invoiceRepository.GetAsync(invoicePageId, cancellationToken))
                .ReturnsAsync((InvoicePage)null);

            var actualInvoicePage = await target.GetImageLinkAsync(invoicePageId, cancellationToken);

            Assert.AreEqual(actualInvoicePage, null);
        }

        [TestMethod]
        [DataRow(10, 1)]
        public async Task GetListAsync_WhenInvoiceIdIsCorrect_ShouldReturnInvoicePages(int count, int invoiceId)
        {
            var invoicePages = Enumerable
                .Range(1, count)
                .Select(index => CreateInvoicePage(index))
                .ToList();

            invoicePageRepositoryMock
                .Setup(invoicePageRepository => invoicePageRepository.GetListAsync(invoiceId, cancellationToken))
                .ReturnsAsync(invoicePages);

            var actualInvoicePages = await target.GetListAsync(invoiceId, cancellationToken);

            for (var index = 1; index <= count; index++)
            {
                AssertInvoicePagesAreEqual(invoicePages[index - 1], actualInvoicePages[index - 1]);
            }

            Assert.AreEqual(invoicePages.Count, actualInvoicePages.Count);
        }

        [TestMethod]
        public async Task CreateAsync_WhenListIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_ShouldCall_CreateAsyncRepositoryMethod()
        {
            var pagesToSave = new List<InvoicePage>();
            pagesToSave.Add(CreateInvoicePage(1));
            pagesToSave.Add(CreateInvoicePage(2));
            invoicePageRepositoryMock
                .Setup(invoicePageRepository => invoicePageRepository.CreateAsync(pagesToSave, cancellationToken))
                .Returns(Task.CompletedTask);
            
            await target.CreateAsync(pagesToSave, CancellationToken.None);
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

        private void AssertInvoicePagesAreEqual(InvoicePage expected, InvoicePage actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ImageFileId, actual.ImageFileId);
            Assert.AreEqual(expected.InvoiceId, actual.InvoiceId);
            Assert.AreEqual(expected.Number, actual.Number);
            Assert.AreEqual(expected.Width, actual.Width);
            Assert.AreEqual(expected.Height, actual.Height);

            // Ensure all properties are tested
            Assert.AreEqual(6, actual.GetType().GetProperties().Length);
        }

        private MockRepository mockRepository;
        private Mock<IInvoicePageRepository> invoicePageRepositoryMock;
        private Mock<IDocumentApiClient> documentApiClientMock;
        private InvoicePageService target;
        private CancellationToken cancellationToken;
    }
}
