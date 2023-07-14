using IdentityServer4.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Definitions;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Configurations
{
    public static class ScopesConfiguration
    {
        public static List<ApiScope> ApiScopes => new List<ApiScope> { invoiceManagementScope, tenantManagementScope, ocrDataAnalysisScope };

        private static readonly ApiScope invoiceManagementScope = new ApiScope(Scopes.InvoiceManagement,
            new List<string>
            {
                InvoiceManagementClaims.TenantId,
                InvoiceManagementClaims.Culture
            });

        private static readonly ApiScope tenantManagementScope = new ApiScope(Scopes.TenantManagement);

        private static readonly ApiScope ocrDataAnalysisScope = new ApiScope(Scopes.OcrDataAnalysis);
    }
}
