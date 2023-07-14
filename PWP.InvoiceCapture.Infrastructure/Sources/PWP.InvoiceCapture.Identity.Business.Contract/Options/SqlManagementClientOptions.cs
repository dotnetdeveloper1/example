namespace PWP.InvoiceCapture.Identity.Business.Contract.Options
{
    public class SqlManagementClientOptions
    {
        public int CommandTimeoutInSeconds { get; set; }
        public string MasterConnectionString { get; set; }
        public string DefaultDatabaseName { get; set; }
    }
}
