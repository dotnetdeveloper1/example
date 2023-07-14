using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class UniqueNameGenerator: INameGenerator
    {
        public string GenerateName()
        {
            return Guid.NewGuid().ToString().Replace(dash, string.Empty);
        }

        private const string dash = "-";
    }
}
