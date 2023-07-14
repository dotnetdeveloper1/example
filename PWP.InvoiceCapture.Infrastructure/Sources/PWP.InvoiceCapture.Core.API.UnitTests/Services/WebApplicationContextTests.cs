using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.API.Services;
using PWP.InvoiceCapture.Core.API.UnitTests.Fakes;
using PWP.InvoiceCapture.Identity.Business.Contract.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;

namespace PWP.InvoiceCapture.Core.API.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WebApplicationContextTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            httpContext = new FakeHttpContext();
            httpContextAccessorMock = mockRepository.Create<IHttpContextAccessor>();
            identityMock = mockRepository.Create<IIdentity>();
            claimsPrincipal = new FakeClaimsPrincipal(identityMock.Object, claims);

            target = new WebApplicationContext(httpContextAccessorMock.Object);
        }

        [TestMethod]
        public void Instance_WhenHttpContextAccessorIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => new WebApplicationContext(null));
        }

        [TestMethod]
        public void TenantId_Get_WhenUserIsNotAuthenticated_ShouldReturnNull()
        {
            SetupHttpContextAccessorMock();
            SetupIdentityIsAuthenticated(false);

            var tenantId = target.TenantId;

            Assert.IsNull(tenantId);
        }

        [TestMethod]
        public void TenantId_Get_WhenUserIsAuthenticated_ShouldReturnTenantId()
        {
            SetupHttpContextAccessorMock();
            SetupIdentityIsAuthenticated(true);

            var tenantId = target.TenantId;

            Assert.IsNotNull(tenantId);
            Assert.AreEqual(expectedTenantId, tenantId);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("tenantId")]
        public void TenantId_Set_ShouldThrowNotSupportedException(string tenantId)
        {
            Assert.ThrowsException<NotSupportedException>(() => target.TenantId = tenantId);
        }

        [TestMethod]
        public void Culture_Get_WhenUserIsNotAuthenticated_ShouldReturnNull()
        {
            SetupHttpContextAccessorMock();
            SetupIdentityIsAuthenticated(false);

            var culture = target.Culture;

            Assert.IsNull(culture);
        }

        [TestMethod]
        public void Culture_Get_WhenUserIsAuthenticated_ShouldReturnCulture()
        {
            SetupHttpContextAccessorMock();
            SetupIdentityIsAuthenticated(true);

            var culture = target.Culture;

            Assert.IsNotNull(culture);
            Assert.AreEqual(expectedCulture, culture);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("en-Us")]
        public void Culture_Set_ShouldThrowNotSupportedException(string culture)
        {
            Assert.ThrowsException<NotSupportedException>(() => target.Culture = culture);
        }

        private void SetupIdentityIsAuthenticated(bool isAuthenticated) 
        {
            identityMock
                .Setup(identity => identity.IsAuthenticated)
                .Returns(isAuthenticated);
        }

        private void SetupHttpContextAccessorMock() 
        {
            httpContextAccessorMock
                .Setup(httpContextAccessor => httpContextAccessor.HttpContext)
                .Returns(httpContext);

            httpContext.User = claimsPrincipal;
        }

        private readonly IEnumerable<Claim> claims = new List<Claim> 
        { 
            new Claim(InvoiceManagementClaims.TenantId, expectedTenantId), 
            new Claim(InvoiceManagementClaims.Culture, expectedCulture) 
        };

        private const string expectedTenantId = "123456";
        private const string expectedCulture = "en-Us";
        private MockRepository mockRepository;
        private Mock<IHttpContextAccessor> httpContextAccessorMock;
        private ClaimsPrincipal claimsPrincipal;
        private Mock<IIdentity> identityMock;
        private HttpContext httpContext;
        private WebApplicationContext target;
    }
}
