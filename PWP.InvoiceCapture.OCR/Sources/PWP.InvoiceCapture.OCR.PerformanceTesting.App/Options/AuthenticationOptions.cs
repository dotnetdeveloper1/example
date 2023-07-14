using PWP.InvoiceCapture.Core.Models;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Options
{
    internal class AuthenticationOptions : ApiClientOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int TenantId { get; set; }
    }
}
