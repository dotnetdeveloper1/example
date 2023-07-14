
namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class PlanCreationParameters
    {
        public string PlanName { get; set; }
        public int TypeId { get; set; }
        public int CurrencyId { get; set; }
        public int AllowedDocumentsCount { get; set; }
        public decimal Price { get; set; }
    }
}
