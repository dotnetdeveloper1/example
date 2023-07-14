using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using System;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class InvoiceProcessingResult
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string VendorName { get; set; }
        public InvoiceProcessingType ProcessingType { get; set; }
        public string TemplateId { get; set; }
        public int TrainingFileCount { get; set; }
        public string DataAnnotationFileId { get; set; }
        public string InitialDataAnnotationFileId { get; set; }
        public string CultureName { get;set;}
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Invoice Invoice { get; set; }
        public DataAnnotation DataAnnotation { get; set; }
    }
}
