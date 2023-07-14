using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class Key
    {
        public string Text { get; set; }
        public List<double?> BoundingBox { get; set; }
        public List<string> Elements { get; set; }
    }
}
