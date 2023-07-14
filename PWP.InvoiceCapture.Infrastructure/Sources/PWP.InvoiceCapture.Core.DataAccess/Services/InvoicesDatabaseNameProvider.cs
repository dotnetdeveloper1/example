using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Core.Utilities;

namespace PWP.InvoiceCapture.Core.DataAccess.Services
{
    public class InvoicesDatabaseNameProvider : IInvoicesDatabaseNameProvider
    {
        public string Get(string tenantId)
        {
            Guard.IsNotNullOrWhiteSpace(tenantId, nameof(tenantId));

            return $"Invoices_{tenantId.Trim()}";
        }
    }
}
