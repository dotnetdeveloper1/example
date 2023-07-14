namespace PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer
{
    public class TrainModelResponse : OCRProviderResponse
    {
        public TrainModelResponse(OCRProviderResponse baseResponse) : base(baseResponse)
        {

        }
        public TrainModelResponse()
        {

        }
        public ModelInfo ModelInfo { get; set; }
        public TrainResult TrainResult { get; set; }
    }

}
