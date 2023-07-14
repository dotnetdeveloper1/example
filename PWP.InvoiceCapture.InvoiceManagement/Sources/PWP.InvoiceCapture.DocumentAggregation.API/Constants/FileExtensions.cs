using System.Collections.Generic;

namespace PWP.InvoiceCapture.DocumentAggregation.API.Constants
{
    public static class FileExtensions
    {
        public static readonly List<string> AllowedExtensions = new List<string> { ".bmp", ".tif", ".tiff", ".jpeg", ".jpg", ".png", ".pdf" };
    }
}
