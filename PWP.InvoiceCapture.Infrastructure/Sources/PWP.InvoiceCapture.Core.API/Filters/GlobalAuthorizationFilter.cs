using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace PWP.InvoiceCapture.Core.API.Filters
{
    public class GlobalAuthorizationFilter
    {
        public static void Register(MvcOptions options)
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.Filters.Add(new AuthorizeFilter(policy));
        }
    }
}
