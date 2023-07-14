namespace PWP.InvoiceCapture.Identity.DataAccess.Contracts
{
    internal interface ITenantsDatabaseContextFactory
    {
        ITenantsDatabaseContext Create();
        ITenantsDatabaseContext CreateWithTracking();
    }
}
