using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.DataAccess.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.DataAccess.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoicesDatabaseNameProviderTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new InvoicesDatabaseNameProvider();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Get_WhenTenantIdIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string tenantId)
        {
            Assert.ThrowsException<ArgumentNullException>(() => target.Get(tenantId));
        }

        [TestMethod]
        [DataRow("1")]
        [DataRow("2")]
        [DataRow("3")]
        [DataRow("tenantId")]
        public void Get_WhenTenantIdIsCorrect_ShouldReturnDatabaseName(string tenantId)
        {
            var expectedResult = $"Invoices_{tenantId}";
            var actualResult = target.Get(tenantId);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Get_WhenTenantIdHasWhiteSpaces_ShouldReturnDatabaseNameWithTrimmedTenantId()
        {
            var tenantId = "  tenantId  ";
            var expectedResult = "Invoices_tenantId";
            var actualResult = target.Get(tenantId);

            Assert.AreEqual(expectedResult, actualResult);
        }

        private InvoicesDatabaseNameProvider target;
    }
}
