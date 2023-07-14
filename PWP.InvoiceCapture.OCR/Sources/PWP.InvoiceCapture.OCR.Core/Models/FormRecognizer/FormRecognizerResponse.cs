using Azure.AI.FormRecognizer.Models;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer;
using System;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models
{
    public class FormRecognizerResponse : OCRProviderResponse
    {
        public FormRecognizerResponse(OCRProviderResponse toCopy) : base(toCopy) { }
        public FormRecognizerResponse() { }
        public bool IsSucceededStatus => Status.Equals(successStatus);
        public string Status { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public AnalyzeResult AnalyzeResult { get; set; }
        public Dictionary<string, FormField> InvoiceFields { get; set; }

        private readonly string successStatus = "succeeded";
    }
}
