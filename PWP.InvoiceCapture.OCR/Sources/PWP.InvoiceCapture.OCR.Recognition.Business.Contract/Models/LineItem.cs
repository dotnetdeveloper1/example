using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class LineItem
    {
        public string Value { get; set; }
        public int PageNo { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }
        public List<int> WordIds { get; set; }
    }
}
