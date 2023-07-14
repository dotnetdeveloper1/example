using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class SqlDatabase
    {
        public string Name { get; set;  }
        public SqlDatabaseState State { get; set; }
        public string StateDescription { get; set; }
    }
}
