using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Extensions;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    public class ImageService : IImageService
    {
        public ImageService(IOptions<ImageConversionOptions> optionsAccessor, ICalculationService calculationService)
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(calculationService, nameof(calculationService));
            GuardConversionParameters(optionsAccessor.Value);

            this.calculationService = calculationService;
            pdfOptions = optionsAccessor.Value;
            imageFormat = pdfOptions.ImageFormat.ToImageFormat();
        }

        public List<PageImage> ConvertToDefaultFormatImages(byte[] bytes)
        {
            Guard.IsNotNull(bytes, nameof(bytes));

            var images = new List<PageImage>();

            using (var sourceImageStream = new MemoryStream(bytes))
            {
                var image = Image.FromStream(sourceImageStream);

                var resizedImage = ResizeImage(image);

                images.Add(ConvertPageImage(resizedImage, imageFormat));
            }
            
            return images;
        }

        private PageImage ConvertPageImage(Bitmap image, ImageFormat imageFormat)
        {
            var convertedBitmap = ConvertBitmapFormat(image, imageFormat);
            
            return CreatePageImage(convertedBitmap, firstPageNumber);
        }

        private Bitmap ConvertBitmapFormat(Bitmap image, ImageFormat imageFormat)
        {
            using (var imageStream = new MemoryStream())
            {
                image.Save(imageStream, imageFormat);
                return new Bitmap(imageStream);
            }
        }

        private PageImage CreatePageImage(Bitmap image, int pageNumber)
        {
            using (var imageStream = new MemoryStream())
            {
                image.Save(imageStream, image.RawFormat);

                return new PageImage(
                            imageStream.ToArray(),
                            pageNumber,
                            image.Width,
                            image.Height,
                            pdfOptions?.ImageFormat);
            }
        }

        private Bitmap ResizeImage(Image image)
        {
            try
            {
                var height = calculationService.CalculateHeight(pdfOptions.Width, image.Size);

                var destRect = new Rectangle(0, 0, pdfOptions.Width, height);
                var destImage = new Bitmap(pdfOptions.Width, height);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return destImage;
            }
            catch
            {
                return new Bitmap(image);
            }
        }

        private void GuardConversionParameters(ImageConversionOptions options)
        {
            Guard.IsNotZeroOrNegative(options.Width, nameof(options.Width));
            Guard.IsNotNullOrWhiteSpace(options.ImageFormat, nameof(options.ImageFormat));
        }

        private readonly ImageConversionOptions pdfOptions;
        private readonly ImageFormat imageFormat;
        private readonly ICalculationService calculationService;

        private const int firstPageNumber = 1;
    }
}
