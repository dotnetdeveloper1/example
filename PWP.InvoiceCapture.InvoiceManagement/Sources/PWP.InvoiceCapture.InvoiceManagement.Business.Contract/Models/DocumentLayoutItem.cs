namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class DocumentLayoutItem
    {
        public string Id { get; set; }
        public int PageNumber { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public DocumentLayoutPoint TopLeft { get; set; }
        public DocumentLayoutPoint BottomRight { get; set; }
    }
}
