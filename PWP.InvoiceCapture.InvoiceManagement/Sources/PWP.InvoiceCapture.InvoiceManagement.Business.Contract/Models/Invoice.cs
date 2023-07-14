using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using System;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string FileName { get; set; }
        public string FileId { get; set; }
        public string ValidationMessage { get; set; }
        public string FromEmailAddress { get; set; }
        public InvoiceState InvoiceState { get; set; }
        public FileSourceType FileSourceType { get; set; }
        public List<InvoiceLine> InvoiceLines { get; set; }
        public List<InvoiceField> InvoiceFields { get; set; }
    }
}
