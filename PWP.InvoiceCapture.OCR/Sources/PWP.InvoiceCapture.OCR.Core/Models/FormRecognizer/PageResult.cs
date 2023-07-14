using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class PageResult
    {
        public int Page { get; set; }
        public List<Table> Tables { get; set; }
        public List<KeyValuePair> KeyValuePairs { get; set; }
        public int ClusterId { get; set; }
    }
}
