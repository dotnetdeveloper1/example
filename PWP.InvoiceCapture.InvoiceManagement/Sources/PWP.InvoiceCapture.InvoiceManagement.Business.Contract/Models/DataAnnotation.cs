using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class DataAnnotation
    {
        public string PlainDocumentText { get; set; }
        public List<DocumentLayoutItem> DocumentLayoutItems { get; set; }
        public List<Annotation> InvoiceAnnotations { get; set; }
        public List<LineAnnotation> InvoiceLineAnnotations { get; set; }
        public List<Table> Tables { get; set; }
    }
}
