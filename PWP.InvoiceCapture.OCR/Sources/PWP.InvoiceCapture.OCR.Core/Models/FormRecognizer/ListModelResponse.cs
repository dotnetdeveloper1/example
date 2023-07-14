using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer
{
    public class ListModelResponse : OCRProviderResponse
    {
        public ListModelResponse(OCRProviderResponse baseResponse) : base(baseResponse)
        {

        }
        public ListModelResponse()
        {

        }
        public ModelListSummary Summary { get; set; }
        public List<ModelInfo> ModelList { get; set; }
        public string NextLink { get; set; }
        
    }
}
