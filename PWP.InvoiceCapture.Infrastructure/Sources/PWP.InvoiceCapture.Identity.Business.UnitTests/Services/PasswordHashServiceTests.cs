using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PasswordHashServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new PasswordHashService();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("     ")]
        [DataRow(null)]
        public void GetHash_WhenPasswordIsNullOrWhitespace_ShouldThrowArgumentException(string password)
        {
            Assert.ThrowsException<ArgumentNullException>(() => target.GetHash(password));
        }

        [TestMethod]
        [DataRow("password")]
        public void GetHash_WhenPasswordIsNullOrWhitespace_ShouldReturnHash(string password)
        {
            var hash = target.GetHash(password);

            Assert.IsNotNull(hash);
            Assert.IsTrue(hash.Length > 0);
        }

        private PasswordHashService target;
    }
}
