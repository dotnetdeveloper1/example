using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class Line
    {
        public string Language { get; set; }
        public List<double> BoundingBox { get; set; }
        public string Text { get; set; }
        public List<Word> Words { get; set; }
    }
}
