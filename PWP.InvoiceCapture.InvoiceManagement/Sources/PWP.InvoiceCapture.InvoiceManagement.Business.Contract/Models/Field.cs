using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using System;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class Field
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsProtected { get; set; }
        public string DisplayName { get; set; }
        public int GroupId { get; set; }
        public int OrderNumber { get; set; }
        public FieldType Type { get; set; }
        public TargetFieldType? TargetFieldType { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string Formula { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
