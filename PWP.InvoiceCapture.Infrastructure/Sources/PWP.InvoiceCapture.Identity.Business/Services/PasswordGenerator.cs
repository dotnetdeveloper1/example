using IdentityServer4.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class PasswordGenerator: IPasswordGenerator
    {
        public string GeneratePassword()
        {
            return Guid.NewGuid().ToString().Sha512();
        }
    }
}
