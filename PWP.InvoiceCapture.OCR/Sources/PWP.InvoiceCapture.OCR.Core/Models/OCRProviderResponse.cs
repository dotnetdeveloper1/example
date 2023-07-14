namespace PWP.InvoiceCapture.OCR.Core.Models
{
    public class OCRProviderResponse
    {
        public OCRProviderResponse()
        {
        }

        public OCRProviderResponse(OCRProviderResponse toCopy)
        {
            ResponseDocument = toCopy.ResponseDocument;
        }

        public string ResponseDocument { get; set; }
    }
}
