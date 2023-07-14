namespace PWP.InvoiceCapture.Identity.Business.Contract.Options
{
    public class LongTermSqlServerBackupOptions
    {
        public bool Enabled { get; set; }
        public string ResourceGroupName { get; set; }
        public string SubscriptionId { get; set; }
        public string TenantId { get; set; }
        public string SqlServerName { get; set; }
        public string WeeklyRetention { get; set; }
        public string MonthlyRetention { get; set; }
        public string YearlyRetention { get; set; }
        public int WeekOfYear { get; set; }
    }
}
