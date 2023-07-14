namespace PWP.InvoiceCapture.Identity.DataAccess.Contracts
{
    internal interface IMasterDatabaseContextFactory
    {
        IMasterDatabaseContext Create();
    }
}
