using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Core.Models;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models
{
    public class RecognitionEngineResponse
    {
        public DataAnnotation DataAnnotation { get; set; }
        public InvoiceTemplate InvoiceTemplate { get; set; }
    }
}
