using System;
using System.Collections.Generic;
using System.Text;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Models
{
    public class AnnotationValue
    {
        public int Page { get; set; }
        public string Text { get; set; }
        public List<List<float>> BoundingBoxes { get; set; }
    }
}
