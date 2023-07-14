using System;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class GroupPlan
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public int GroupId { get; set; }
        public int UploadedDocumentsCount { get; set; }
        public bool IsRenewalCancelled { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Plan Plan { get; set; }
        public Group Group { get; set; }
    }
}
