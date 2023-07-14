using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using System;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DatabaseName { get; set; }
        public bool IsActive { get; set; }
        public int GroupId { get; set; }
        public TenantDatabaseStatus Status { get; set; }
        public string DocumentUploadEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public void SetCopiedState()
        {
            IsActive = true;
            Status = TenantDatabaseStatus.Copied;
        }

        public void SetFailedState()
        {
            IsActive = false;
            Status = TenantDatabaseStatus.FailedToCopy;
        }

        public void SetNotCopiedState()
        {
            IsActive = false;
            Status = TenantDatabaseStatus.NotCopied;
        }
    }
}
