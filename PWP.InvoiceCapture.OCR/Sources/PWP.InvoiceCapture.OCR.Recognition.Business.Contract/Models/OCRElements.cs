using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class OCRElements
    {
        public IEnumerable<Line> Lines { get; set; }
        public IEnumerable<WordDefinition> Words { get; set; }
        public OCRProviderRecognitionData OCRProviderRecognitionData {get;set;}
        public string RawText { get; set; }
    }
}
