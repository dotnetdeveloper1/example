using System;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class FieldGroup
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsProtected { get; set; }
        public string DisplayName { get; set; }
        public int OrderNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<Field> Fields { get; set; }
    }
}
