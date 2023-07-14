using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SymmetricSigningCredentialStoreTests
    {
        [TestInitialize]
        public void Initialize()
        {
            var options = new AuthenticationServerOptions { SigningKey = signingKey };
            var optionsAccessor = Options.Create(options);

            target = new SymmetricSigningCredentialStore(optionsAccessor);
        }

        [TestMethod]
        public void Instance_WhenAuthenticationServerOptionsAccessorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SymmetricSigningCredentialStore(null));
        }

        [TestMethod]
        public void Instance_WhenAuthenticationServerOptionsAreNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<AuthenticationServerOptions>(null);

            Assert.ThrowsException<ArgumentNullException>(() => new SymmetricSigningCredentialStore(optionsAccessor));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenSigningKeyIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string signingKey)
        {
            var options = new AuthenticationServerOptions 
            {
                SigningKey = signingKey
            };

            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() => new SymmetricSigningCredentialStore(optionsAccessor));
        }

        [TestMethod]
        public async Task GetSigningCredentialsAsync_ShouldReturnHmacSha512SymmetricKey() 
        {
            var actualResult = await target.GetSigningCredentialsAsync();

            Assert.IsNotNull(actualResult);
            Assert.AreEqual(SecurityAlgorithms.HmacSha512, actualResult.Algorithm);
            Assert.IsNotNull(actualResult.Key);
            Assert.IsInstanceOfType(actualResult.Key, typeof(SymmetricSecurityKey));
        }
        
        [TestMethod]
        public async Task GetValidationKeysAsync_ShouldReturnHmacSha512SymmetricKey() 
        {
            var actualResult = await target.GetValidationKeysAsync();

            Assert.IsNotNull(actualResult);
            Assert.AreEqual(1, actualResult.Count());

            var validationKey = actualResult.Single();

            Assert.AreEqual(SecurityAlgorithms.HmacSha512, validationKey.SigningAlgorithm);
            Assert.IsNotNull(validationKey.Key);
            Assert.IsInstanceOfType(validationKey.Key, typeof(SymmetricSecurityKey));
        }

        private const string signingKey = "1be28d911aa4d790eb433d55f9697fbefc0eed27b5893ef5baf6ab1642256ab5dd02888b08bafed86e15ffcdbf931eeb278418368edbc1861d95ed21214868fc";
        private SymmetricSigningCredentialStore target;
    }
}
