using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using System;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class Webhook
    {
        public int Id { get; set; }
        public TriggerType TriggerType { get; set; }
        public string Url { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
