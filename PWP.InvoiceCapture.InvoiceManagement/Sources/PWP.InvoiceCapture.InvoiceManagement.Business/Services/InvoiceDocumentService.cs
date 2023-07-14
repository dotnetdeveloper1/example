using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    public class InvoiceDocumentService: IInvoiceDocumentService
    {
        public InvoiceDocumentService(IImageService imageService, IPdfService pdfService, IDocumentApiClient documentApiClient)
        {
            Guard.IsNotNull(pdfService, nameof(pdfService));
            Guard.IsNotNull(imageService, nameof(imageService));
            Guard.IsNotNull(documentApiClient, nameof(documentApiClient));

            this.pdfService = pdfService;
            this.imageService = imageService;
            this.documentApiClient = documentApiClient;
        }

        public async Task<List<InvoicePage>> GetInvoicePagesAsync(int invoiceId, string fileId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(fileId, nameof(fileId));
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            if (IsPdf(fileId))
            {
                return await GetPdfPagesAsync(invoiceId, fileId, cancellationToken);
            }

            return await GetImagePagesAsync(invoiceId, fileId, cancellationToken);
        }

        private async Task<List<InvoicePage>> GetImagePagesAsync(int invoiceId, string fileId, CancellationToken cancellationToken)
        {
            using (var imageStream = await documentApiClient.GetDocumentStreamAsync(fileId, cancellationToken))
            {
                var imageBytes = GetBytes(imageStream);

                var pagesTasks = imageService
                        .ConvertToDefaultFormatImages(imageBytes)
                        .Select(page => UploadImageAsync(page, invoiceId, cancellationToken));

                var result = await Task.WhenAll(pagesTasks);

                return result.ToList();
            }
        }

        private async Task<List<InvoicePage>> GetPdfPagesAsync(int invoiceId, string fileId, CancellationToken cancellationToken)
        {
            using (var invoiceDocumentStream = await documentApiClient.GetDocumentStreamAsync(fileId, cancellationToken))
            {
                var pdfBytes = GetBytes(invoiceDocumentStream);

                var pagesTasks = pdfService
                        .ConvertToImages(pdfBytes)
                        .Select(page => UploadImageAsync(page, invoiceId, cancellationToken));

                var result = await Task.WhenAll(pagesTasks);

                return result.ToList();
            }
        }

        private bool IsPdf(string fileId)
        {
            var fileExtension = Path.GetExtension(fileId);

            return string.Equals(pdfExtension, fileExtension.ToLower());
        }

        private async Task<InvoicePage> UploadImageAsync(PageImage pageImage, int invoiceId, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream(pageImage.ImageData))
            {
                var document = await documentApiClient.UploadDocumentAsync(
                    stream,
                    $"{pageImage.PageNumber}.{pageImage.ImageFormat}",
                    cancellationToken);

                return new InvoicePage()
                {
                    InvoiceId = invoiceId,
                    Number = pageImage.PageNumber,
                    ImageFileId = document.Data.FileId,
                    Height = pageImage.Height,
                    Width = pageImage.Width
                };
            }
        }

        private byte[] GetBytes(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private readonly IImageService imageService;
        private readonly IPdfService pdfService;
        private readonly IDocumentApiClient documentApiClient;
        private const string pdfExtension = ".pdf";
        private const string defaultImageFormat = ".png";
    }
}
