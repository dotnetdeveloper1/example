using System.Collections.Generic;

namespace PWP.InvoiceCapture.Core.API.Models
{
    public class InvoiceCaptureAuthenticationOptions
    {
        public string SigningKey { get; set; }
        public bool AllowInsecureHttp { get; set; }
        public IEnumerable<string> ValidAudiences { get; set; }
    }
}
