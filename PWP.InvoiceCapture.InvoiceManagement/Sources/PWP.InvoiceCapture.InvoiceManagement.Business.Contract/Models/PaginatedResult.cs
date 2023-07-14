using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    //TODO: Move to Core package
    public class PaginatedResult<TResult>
    {
        public int TotalItemsCount { get; set; }
        public List<TResult> Items { get; set; }
    }
}
