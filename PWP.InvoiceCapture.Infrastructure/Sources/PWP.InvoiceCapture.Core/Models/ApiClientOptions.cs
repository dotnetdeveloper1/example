namespace PWP.InvoiceCapture.Core.Models
{
    public class ApiClientOptions
    {
        public string BaseAddress { get; set; }
        public int RetryAttemptCount { get; set; } = 5;
        public int TimeoutInSeconds { get; set; } = 30;
    }
}
