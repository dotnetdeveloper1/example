using System;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class InvoiceLine
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int OrderNumber { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
