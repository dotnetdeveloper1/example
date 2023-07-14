using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer
{
    public class AnalyzeResult
    {
        public string Version { get; set; }
        public List<ReadResult> ReadResults { get; set; }
        public List<PageResult> PageResults { get; set; }
        public List<object> Errors { get; set; }
        public List<DocumentResult> DocumentResults { get; set; }
    }
}
