namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    public class InvoicePage
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int Number { get; set; }
        public string ImageFileId { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
