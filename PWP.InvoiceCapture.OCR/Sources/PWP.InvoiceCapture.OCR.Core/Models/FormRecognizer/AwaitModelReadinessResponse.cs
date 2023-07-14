using System;
using System.Collections.Generic;
using System.Text;

namespace PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer
{
    public class AwaitModelReadinessResponse : OCRProviderResponse
    {
        public AwaitModelReadinessResponse(OCRProviderResponse toCopy) : base(toCopy) { }
        public AwaitModelReadinessResponse() { }
        public bool IsModelReady { get; set; }
        public TrainModelResponse ModelDetails { get; set; }
    }
}
