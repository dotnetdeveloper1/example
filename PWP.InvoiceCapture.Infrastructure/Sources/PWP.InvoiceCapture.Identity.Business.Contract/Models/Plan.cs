using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using System;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PlanType Type { get; set; }
        public int AllowedDocumentsCount { get; set; }
        public decimal Price { get; set; }
        public int CurrencyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Currency Currency { get; set; }

    }
}
