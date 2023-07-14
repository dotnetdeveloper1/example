namespace PWP.InvoiceCapture.OCR.Core.Models
{
    public class DocumentStorageOptions
    {
        public int BlobRetryIntervalInSeconds { get; set; }
        public int BlobRetryAttempts { get; set; }
        public string BlobConnectionString { get; set; }
        public int LinkTimeToLiveInSeconds { get; set; }
    }
}
