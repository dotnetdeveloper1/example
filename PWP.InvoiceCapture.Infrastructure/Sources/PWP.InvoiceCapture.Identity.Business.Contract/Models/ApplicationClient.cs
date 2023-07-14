using System;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class ApplicationClient
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string SecretHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
