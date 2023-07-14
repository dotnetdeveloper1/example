using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;

namespace PWP.InvoiceCapture.Core.API.UnitTests.Fakes
{
    [ExcludeFromCodeCoverage]
    internal class FakeClaimsPrincipal : ClaimsPrincipal
    {
        public FakeClaimsPrincipal(IIdentity identity, IEnumerable<Claim> claims) : base() 
        {
            this.claims = claims;
            this.identity = identity;
        }

        public override IEnumerable<Claim> Claims => claims;

        public override IIdentity Identity => identity;

        private readonly IEnumerable<Claim> claims;
        private readonly IIdentity identity;
    }
}
