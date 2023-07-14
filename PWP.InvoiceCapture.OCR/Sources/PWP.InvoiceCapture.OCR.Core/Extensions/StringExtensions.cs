using System.IO;
using System.Text;

namespace PWP.InvoiceCapture.OCR.Core.Extensions
{
    public static class StringExtensions
    {
        public static MemoryStream ToStream(this string input)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(input));
        }

    }
}
