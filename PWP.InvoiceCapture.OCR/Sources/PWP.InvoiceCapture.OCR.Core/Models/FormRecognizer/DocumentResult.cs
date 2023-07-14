using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class DocumentResult
    {
        public string DocType { get; set; }
        public List<int> PageRange { get; set; } = new List<int>();
        public Dictionary<string, DocumentResultItem> Fields { get; set; } = new Dictionary<string, DocumentResultItem>();
    }
}
