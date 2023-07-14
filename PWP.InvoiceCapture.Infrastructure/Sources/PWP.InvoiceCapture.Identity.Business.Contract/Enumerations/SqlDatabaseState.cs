namespace PWP.InvoiceCapture.Identity.Business.Contract.Enumerations
{
    //https://docs.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-databases-transact-sql
    public enum SqlDatabaseState: byte
    {
        Online = 0,
        Restoring = 1,
        Recovering = 2,
        RecoveryPending = 3,
        Suspect = 4,
        Emergency = 5,
        Offline = 6,
        Copying = 7,
        OfflineSecondary = 10
    }
}
