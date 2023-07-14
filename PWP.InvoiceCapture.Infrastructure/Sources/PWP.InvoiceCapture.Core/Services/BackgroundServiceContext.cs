using PWP.InvoiceCapture.Core.Contracts;
using System.Threading;

namespace PWP.InvoiceCapture.Core.Services
{
    public class BackgroundServiceContext : IApplicationContext
    {
        public string TenantId 
        { 
            get => tenantId.Value; 
            set => tenantId.Value = value; 
        }

        public string Culture 
        {
            get => culture.Value;
            set => culture.Value = value; 
        }

        private static readonly AsyncLocal<string> tenantId = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> culture = new AsyncLocal<string>();
    }
}
