using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class DocumentResultItem
    {
        public string Type { get; set; }
        public string ValueString { get; set; }
        public string Text { get; set; }
        public List<float> BoundingBox { get; set; }
        public double Confidence { get; set; }
        public int Page { get; set; }
        public List<string> Elements { get; set; }
    }
}
