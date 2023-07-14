using System;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class InvoiceTemplateCultureSetting
    {
        public int Id { get; set; }
        public string CultureName { get; set; }
        public string TemplateId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
