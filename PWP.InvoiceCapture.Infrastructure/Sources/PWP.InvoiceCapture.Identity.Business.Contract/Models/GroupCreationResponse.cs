namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class GroupCreationResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentGroupId { get; set; }
        public UserCreationResponse DefaultUser { get; set; }
    }
}
