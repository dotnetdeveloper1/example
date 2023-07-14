using System;
using System.Collections.Generic;
using System.Text;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentGroupId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<Tenant> Tenants { get; set; }
    }
}
