using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class Cell
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public string Text { get; set; }
        public List<float> BoundingBox { get; set; }
        public List<string> Elements { get; set; }
        public int? ColumnSpan { get; set; }
        public int? RowSpan { get; set; }
    }
}
