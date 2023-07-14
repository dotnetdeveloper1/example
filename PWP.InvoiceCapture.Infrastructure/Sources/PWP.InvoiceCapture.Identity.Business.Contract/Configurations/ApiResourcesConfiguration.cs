using IdentityServer4.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Definitions;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Configurations
{
    public static class ApiResourcesConfiguration
    {
        public static List<ApiResource> ApiResources = new List<ApiResource>
        {
            new ApiResource()
            {
                Name = Scopes.InvoiceManagement,
                Scopes = new[] { Scopes.InvoiceManagement }
            },
            new ApiResource()
            {
                Name = Scopes.TenantManagement,
                Scopes = new[] { Scopes.TenantManagement }
            },
            new ApiResource()
            {
                Name = Scopes.OcrDataAnalysis,
                Scopes = new[] { Scopes.OcrDataAnalysis }
            }
        };
    }
}
