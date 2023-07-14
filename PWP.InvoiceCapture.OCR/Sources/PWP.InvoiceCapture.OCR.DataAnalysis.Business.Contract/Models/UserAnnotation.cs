using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Models
{
    public class TrainingAnnotation
    {
        public string Label { get; set; }
        public object Key { get; set; }
        public IList<AnnotationValue> Value { get; set; }
    }
}
