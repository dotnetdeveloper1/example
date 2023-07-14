using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer
{
    public class TrainingDocument
    {
        public string DocumentName { get; set; }
        public int Pages { get; set; }
        public string Status { get; set; }
        public IList<string> Errors { get; set; }
    }
}
