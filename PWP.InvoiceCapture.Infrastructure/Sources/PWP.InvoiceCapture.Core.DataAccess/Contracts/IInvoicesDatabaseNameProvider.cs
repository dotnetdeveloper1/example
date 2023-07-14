namespace PWP.InvoiceCapture.Core.DataAccess.Contracts
{
    public interface IInvoicesDatabaseNameProvider
    {
        string Get(string tenantId);
    }
}
