
namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class PackCreationParameters
    {
        public string PackName { get; set; }
        public int CurrencyId { get; set; }
        public int AllowedDocumentsCount { get; set; }
        public decimal Price { get; set; }
    }
}
