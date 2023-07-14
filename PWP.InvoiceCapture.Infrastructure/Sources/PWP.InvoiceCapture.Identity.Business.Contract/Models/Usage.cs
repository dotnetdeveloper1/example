using System.Collections.Generic;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class Usage
    {
        public GroupPlan ActivePlan { get; set; }
        public List<GroupPack> TotalAvailablePacks { get; set; }
    }
}
