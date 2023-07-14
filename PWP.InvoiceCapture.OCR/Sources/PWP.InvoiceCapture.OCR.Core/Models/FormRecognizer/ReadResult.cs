using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class ReadResult
    {
        public int Page { get; set; }
        public string Language { get; set; }
        public float Angle { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Unit { get; set; }
        public List<Line> Lines { get; set; }
    }
}
