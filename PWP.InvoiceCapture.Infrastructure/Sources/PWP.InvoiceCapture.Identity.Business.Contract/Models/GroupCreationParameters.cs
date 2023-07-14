namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class GroupCreationParameters
    {
        public string Name { get; set; }
        public int? ParentGroupId { get; set; }
    }
}
