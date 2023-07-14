using PWP.InvoiceCapture.OCR.Core.Models;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class InitiateAnalysisResult : OCRProviderResponse
    {
        public InitiateAnalysisResult(OCRProviderResponse baseResponse) : base(baseResponse) { }
        public InitiateAnalysisResult() {}
        public string ResourceLocation { get; set; }

    }
}
