namespace PWP.InvoiceCapture.Document.Business.Contract.Models
{
    public class DocumentStorageOptions
    {
        public int BlobRetryIntervalInSeconds { get; set; }
        public int BlobRetryAttempts { get; set; }
        public string BlobConnectionString { get; set; }
        public string BlobContainer { get; set; }
        public int LinkTimeToLiveInSeconds { get; set; }
    }
}
