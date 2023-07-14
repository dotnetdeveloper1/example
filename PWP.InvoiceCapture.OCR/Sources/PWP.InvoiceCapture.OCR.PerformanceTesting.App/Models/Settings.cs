namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models
{
    internal class Settings
    {
        public string Environment { get; set; }
        public string FolderPath { get; set; }
        public int TenantId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int ThreadsCount { get; set; }
        public int InvoicePollingIntervalMilliseconds { get; set; }
        public int InvoiceUploadingIntervalMilliseconds { get; set; }
    }
}
