namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts
{
    internal interface IDatabaseContextFactory
    {
        IDatabaseContext Create();
        IDatabaseContext CreateWithTracking();
    }
}
