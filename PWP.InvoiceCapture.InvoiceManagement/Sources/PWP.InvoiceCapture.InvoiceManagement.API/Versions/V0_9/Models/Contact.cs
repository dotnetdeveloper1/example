using System;

namespace PWP.InvoiceCapture.InvoiceManagement.API.Versions.V0_9.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Name { get; set; }
        public ContactType ContactType { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
    }
}
