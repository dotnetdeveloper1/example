using System;

namespace PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer
{
    public class ModelInfo
    {
        public string ModelId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
    }
}
