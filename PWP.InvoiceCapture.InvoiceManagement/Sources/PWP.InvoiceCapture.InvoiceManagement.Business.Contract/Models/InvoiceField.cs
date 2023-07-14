using System;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class InvoiceField
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int InvoiceId { get; set; }
        public int FieldId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Field Field { get; set; }
    }
}
