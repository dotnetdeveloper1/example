using Microsoft.Extensions.Options;
using PdfiumViewer;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Extensions;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    public class PdfService: IPdfService
    {
        public PdfService(IOptions<ImageConversionOptions> optionsAccessor, ICalculationService calculationService)
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(calculationService, nameof(calculationService));
            GuardConversionParameters(optionsAccessor.Value);

            this.calculationService = calculationService;
            pdfOptions = optionsAccessor.Value;
            imageFormat = pdfOptions.ImageFormat.ToImageFormat();
        }

        public List<PageImage> ConvertToImages(byte[] bytes)
        {
            Guard.IsNotNull(bytes, nameof(bytes));

            var images = new List<PageImage>();

            using (var pdfDocumentStream = new MemoryStream(bytes))
            using (var document = PdfDocument.Load(pdfDocumentStream))
            {
                for (int pageNumber = 1; pageNumber <= document.PageCount; pageNumber++)
                {
                    var pageSize = document.PageSizes[pageNumber - 1];
                    var imageHeight = calculationService.CalculateHeight(pdfOptions.Width, pageSize);
                    var imageSize = new Size(pdfOptions.Width, imageHeight);

                    using (var image = ConvertPdfPageToImage(pageNumber - 1, imageSize, document))
                    using (var imageStream = new MemoryStream())
                    {
                        image.Save(imageStream, imageFormat);

                        images.Add(new PageImage(
                            imageStream.ToArray(),
                            pageNumber,
                            imageSize.Width,
                            imageSize.Height,
                            pdfOptions?.ImageFormat));
                    }
                }
            }

            return images;
        }

        private void GuardConversionParameters(ImageConversionOptions options)
        {
            Guard.IsNotZeroOrNegative(options.Width, nameof(options.Width));
            Guard.IsNotZeroOrNegative(options.Dpi, nameof(options.Dpi));
            Guard.IsNotNullOrWhiteSpace(options.ImageFormat, nameof(options.ImageFormat));
        }

        private Image ConvertPdfPageToImage(int pageIndex, Size size, PdfDocument document)
        {
            return document.Render(pageIndex, size.Width, size.Height, pdfOptions.Dpi, pdfOptions.Dpi, PdfRenderFlags.Annotations);
        }

        private readonly ImageConversionOptions pdfOptions;
        private readonly ICalculationService calculationService;
        private readonly ImageFormat imageFormat;
    }
}
