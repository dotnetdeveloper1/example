using System;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class TenantSetting
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int CultureId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
