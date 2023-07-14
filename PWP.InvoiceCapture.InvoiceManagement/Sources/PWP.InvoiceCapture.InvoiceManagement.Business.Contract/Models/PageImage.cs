using PWP.InvoiceCapture.Core.Utilities;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class PageImage
    {
        public PageImage(byte[] imageData, int pageNumber, int width, int height, string imageFormat)
        {
            Guard.IsNotNull(imageData, nameof(imageData));
            Guard.IsNotZeroOrNegative(pageNumber, nameof(pageNumber));
            Guard.IsNotZeroOrNegative(width, nameof(width));
            Guard.IsNotZeroOrNegative(height, nameof(height));
            Guard.IsNotNullOrWhiteSpace(imageFormat, nameof(imageFormat));

            ImageData = imageData;
            PageNumber = pageNumber;
            Width = width;
            Height = height;
            ImageFormat = imageFormat;
        }

        public byte[] ImageData { get; }
        public int PageNumber { get; }
        public int Width { get; }
        public int Height { get; }
        public string ImageFormat { get; }
    }
}
