using Microsoft.AspNetCore.Http;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Definitions;
using System;
using System.Linq;

namespace PWP.InvoiceCapture.Core.API.Services
{
    public class WebApplicationContext : IApplicationContext
    {
        public WebApplicationContext(IHttpContextAccessor httpContextAccessor)
        {
            Guard.IsNotNull(httpContextAccessor, nameof(httpContextAccessor));

            this.httpContextAccessor = httpContextAccessor;
        }

        public string TenantId
        {
            get => GetClaimValue(InvoiceManagementClaims.TenantId);
            set => throw new NotSupportedException();
        }

        public string Culture 
        { 
            get => GetClaimValue(InvoiceManagementClaims.Culture);
            set => throw new NotSupportedException();
        }

        private string GetClaimValue(string claimType)
        {
            Guard.IsNotNull(claimType, nameof(claimType));

            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext.User?.Identity == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var claims = httpContext.User.Claims;

            return claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
        }

        private readonly IHttpContextAccessor httpContextAccessor;
    }
}
