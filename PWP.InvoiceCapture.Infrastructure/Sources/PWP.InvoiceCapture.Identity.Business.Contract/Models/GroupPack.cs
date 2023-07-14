using System;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class GroupPack
    {
        public int Id { get; set; }
        public int PackId { get; set; }
        public int GroupId { get; set; }
        public int UploadedDocumentsCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public Group Group { get; set; }
        public Pack Pack { get; set; }
    }
}
