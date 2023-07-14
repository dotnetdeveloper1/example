using System;

namespace PWP.InvoiceCapture.OCR.Core.Models
{
    public class FormRecognizerResource
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
