using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.API.Versions.V0_9.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string PONumber { get; set; }
        public string TaxNumber { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? FreightAmount { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public int? CurrencyId { get; set; }
        public string FileName { get; set; }
        public string FileId { get; set; }
        public string ValidationMessage { get; set; }
        public string FromEmailAddress { get; set; }
        public InvoiceState InvoiceState { get; set; }
        public FileSourceType FileSourceType { get; set; }
        public Currency Currency { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<InvoiceLine> InvoiceLines { get; set; }
    }
}
