using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class Annotation
    {
        public string FieldType { get; set; }
        public string FieldValue { get; set; }
        public bool UserCreated { get; set; }
        public List<string> DocumentLayoutItemIds { get; set; }
    }
}
