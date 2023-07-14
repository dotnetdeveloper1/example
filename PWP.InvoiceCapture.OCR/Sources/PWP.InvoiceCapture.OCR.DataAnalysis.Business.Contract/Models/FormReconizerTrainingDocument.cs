using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Models
{
    public class FormReconizerTrainingDocument
    {
        public string Document { get; set; }
        public List<TrainingAnnotation> Labels { get; set; }
    }
}
