using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models
{
    internal class InvoiceGroupRecognitionResult
    {
        public string Folder { get; set; }
        public bool IsSameTemplate => CheckSameTemplate();
        public List<InvoiceRecognitionResult> Results { get; set; }

        private bool CheckSameTemplate() 
        {
            if (Results == null || Results.Count == 0)
            {
                return false;
            }

            var templateId = Results.First().OcrTemplateId;

            return Results.TrueForAll(result => result.OcrTemplateId == templateId);
        }
    }
}
