using System;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class FormulaField
    {
        public int Id { get; set; }
        public int ResultFieldId { get; set; }
        public Field ResultField { get; set; }
        public int OperandFieldId { get; set; }
        public Field OperandField { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
