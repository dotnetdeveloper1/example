using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class Word
    {
        public List<float> BoundingBox { get; set; }
        public string Text { get; set; }
        public double Confidence { get; set; }
        public int InternalId { get; set; }
    }
}
