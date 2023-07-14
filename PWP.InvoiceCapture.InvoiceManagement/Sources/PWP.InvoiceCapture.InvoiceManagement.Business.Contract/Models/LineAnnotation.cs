using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class LineAnnotation
    {
        public int OrderNumber { get; set; }
        public List<Annotation> LineItemAnnotations { get; set; }
    }
}
