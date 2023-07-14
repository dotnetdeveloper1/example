using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.Document.API.Client.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceDocumentServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            cancellationToken = CancellationToken.None;
            mockRepository = new MockRepository(MockBehavior.Strict);
            documentApiClientMock = mockRepository.Create<IDocumentApiClient>();
            imageServiceMock = mockRepository.Create<IImageService>();
            pdfServiceMock = mockRepository.Create<IPdfService>();
            target = new InvoiceDocumentService(imageServiceMock.Object, pdfServiceMock.Object, documentApiClientMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenImageServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceDocumentService(null, null, documentApiClientMock.Object));
        }

        [TestMethod]
        public void Instance_WhenPdfServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceDocumentService(imageServiceMock.Object, null, documentApiClientMock.Object));
        }

        [TestMethod]
        public void Instance_WhenDocumentApiClientIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceDocumentService(imageServiceMock.Object, pdfServiceMock.Object, null));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetInvoicePagesAsync_WhenInvoiceIdIsZeroOrLess_ShouldThrowArgumentException(int invoiceId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetInvoicePagesAsync(invoiceId, filePdfId, cancellationToken));
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow("")]
        [DataRow(null)]
        public async Task GetInvoicePagesAsync_WhenFileIdIsNullOrWhiteSpace_ShouldArgumentNullException(string fileId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.GetInvoicePagesAsync(1, fileId, cancellationToken));
        }

        [TestMethod]
        public async Task GetInvoicePagesAsync_WhenFileIsPdf_ShouldReturnInvoicePages()
        {
            const int invoiceId = 1;

            var bytes = File.ReadAllBytes($"Files/{pdfFileName}.pdf");
            var stream = new MemoryStream(bytes);

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.GetDocumentStreamAsync(filePdfId, cancellationToken))
                .ReturnsAsync(stream);

            var image1 = CreatePageImage(1);
            var image2 = CreatePageImage(2);
            var images = new List<PageImage> { image1, image2 };

            pdfServiceMock
                .Setup(pdfService => pdfService.ConvertToImages(bytes))
                .Returns(images);

            var uploadDocumentResponce1 = GetUploadDocumentResponse(image1);
            var fileName1 = $"{image1.PageNumber}.png";

            var imageStreams = new List<MemoryStream>();

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.UploadDocumentAsync(Capture.In(imageStreams), fileName1, cancellationToken))
                .ReturnsAsync(uploadDocumentResponce1);

            var uploadDocumentResponce2 = GetUploadDocumentResponse(image2);
            var fileName2 = $"{image2.PageNumber}.png";
            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.UploadDocumentAsync(Capture.In(imageStreams), fileName2, cancellationToken))
                .ReturnsAsync(uploadDocumentResponce2);

            var pages = await target.GetInvoicePagesAsync(invoiceId, filePdfId, cancellationToken);

            Assert.AreEqual(imageStreams.Count, 2);
            CheckPageFields(pages);
        }

        [TestMethod]
        public async Task GetInvoicePagesAsync_WhenFileIsPng_ShouldReturnInvoicePages()
        {
            const int invoiceId = 1;

            var bytes = File.ReadAllBytes($"Files/{imageFileName}.png");
            var stream = new MemoryStream(bytes);

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.GetDocumentStreamAsync(filePngId, cancellationToken))
                .ReturnsAsync(stream);
            
            var image1 = CreatePageImage(1);
            var images = new List<PageImage> { image1 };
            imageServiceMock
                .Setup(imageService => imageService.ConvertToDefaultFormatImages(bytes))
                .Returns(images);

            var uploadDocumentResponce1 = GetUploadDocumentResponse(image1);
            var fileName1 = $"{image1.PageNumber}.png";

            var imageStreams = new List<MemoryStream>();

            documentApiClientMock
                .Setup(documentApiClient => documentApiClient.UploadDocumentAsync(Capture.In(imageStreams), fileName1, cancellationToken))
                .ReturnsAsync(uploadDocumentResponce1);

            var pages = await target.GetInvoicePagesAsync(invoiceId, filePngId, cancellationToken);

            Assert.IsTrue(pages.Any(page => page.Number == 1));
            Assert.IsFalse(pages.All(page => string.IsNullOrWhiteSpace(page.ImageFileId)));
            Assert.IsTrue(pages.Any(page => page.InvoiceId == invoiceId));
            Assert.AreEqual(6, pages[0].GetType().GetProperties().Length);
        }

        private void CheckPageFields(List<InvoicePage> pages)
        {
            Assert.IsTrue(pages.All(page => page.Width == width));
            Assert.IsTrue(pages.All(page => page.Height == height));

            Assert.IsTrue(pages.Any(page => page.Number == 1));
            Assert.IsTrue(pages.Any(page => page.Number == 2));

            Assert.IsTrue(pages.Any(page => page.ImageFileId == "1"));
            Assert.IsTrue(pages.Any(page => page.ImageFileId == "2"));

            Assert.IsTrue(pages.Any(page => page.InvoiceId == 1));

            Assert.AreEqual(6, pages[0].GetType().GetProperties().Length);
        }

        private PageImage CreatePageImage(int imageNumber)
        {
            var bytes = File.ReadAllBytes($"Files/{imageFileName}.png");

            return new PageImage(bytes, imageNumber, width, height, "png");
        }

        private ApiResponse<UploadDocumentResponse> GetUploadDocumentResponse(PageImage pdfPageImage)
        {
            var data = new UploadDocumentResponse() { FileId = pdfPageImage.PageNumber.ToString() };

            return new ApiResponse<UploadDocumentResponse>() { Data = data };
        }

        private InvoiceDocumentService target;
        private MockRepository mockRepository;
        private Mock<IDocumentApiClient> documentApiClientMock;
        private Mock<IPdfService> pdfServiceMock;
        private Mock<IImageService> imageServiceMock;
        private CancellationToken cancellationToken;

        private const string filePdfId = "fileId.pdf";
        private const string filePngId = "fileId.png";
        private const string imageFileName = "0";
        private const string pdfFileName = "PdfSample";
        private const int width = 860;
        private const int height = 1280;
    }
}
