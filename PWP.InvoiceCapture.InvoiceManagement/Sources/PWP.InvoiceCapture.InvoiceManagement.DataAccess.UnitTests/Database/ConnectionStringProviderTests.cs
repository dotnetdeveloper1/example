using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.UnitTests.Database
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ConnectionStringProviderTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            databaseNameProviderMock = mockRepository.Create<IInvoicesDatabaseNameProvider>();
            applicationContextMock = mockRepository.Create<IApplicationContext>();
            
            var options = new DatabaseOptions { ConnectionString = connectionString };
            optionsAccessor = Options.Create(options);

            target = new ConnectionStringProvider(databaseNameProviderMock.Object, applicationContextMock.Object, optionsAccessor);
        }

        [TestMethod]
        public void Instance_WhenDatabaseNameProviderIsNull_ShouldThrowArgumentNullException() 
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new ConnectionStringProvider(null, applicationContextMock.Object, optionsAccessor));
        }

        [TestMethod]
        public void Instance_WhenApplicationContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ConnectionStringProvider(databaseNameProviderMock.Object, null, optionsAccessor));
        }

        [TestMethod]
        public void Instance_WhenDatabaseOptionsAccessorIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ConnectionStringProvider(databaseNameProviderMock.Object, applicationContextMock.Object, null));
        }

        [TestMethod]
        public void Instance_WhenDatabaseOptionsIsNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<DatabaseOptions>(null);

            Assert.ThrowsException<ArgumentNullException>(() =>
                new ConnectionStringProvider(databaseNameProviderMock.Object, applicationContextMock.Object, optionsAccessor));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenConnectionStringIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string connectionString)
        {
            var options = new DatabaseOptions { ConnectionString = connectionString };
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() =>
                new ConnectionStringProvider(databaseNameProviderMock.Object, applicationContextMock.Object, optionsAccessor));
        }

        [TestMethod]
        public void Get_WhenTenantIdIsNull_ShouldThrowInvalidOperationException() 
        {
            SetupApplicationContextMock(null);

            Assert.ThrowsException<InvalidOperationException>(() => target.Get());
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Get_WhenDatabaseNameIsNullOrWhiteSpace_ShouldThrowInvalidOperationException(string databaseName)
        {
            SetupApplicationContextMock(tenantId);
            SetupDatabaseNameProviderMock(databaseName, tenantId);

            Assert.ThrowsException<InvalidOperationException>(() => target.Get());
        }

        [TestMethod]
        public void Get_WhenCorrectTenantIdAndDatabaseName_ShouldReturnConnectionString() 
        {
            SetupApplicationContextMock(tenantId);
            SetupDatabaseNameProviderMock(databaseName, tenantId);

            var expectedConnectionString = string.Format(connectionString, databaseName);
            var actualConnectionString = target.Get();

            Assert.IsNotNull(actualConnectionString);
            Assert.AreEqual(expectedConnectionString, actualConnectionString);
        }

        private void SetupApplicationContextMock(string tenantId) 
        {
            applicationContextMock
                .SetupGet(applicationContext => applicationContext.TenantId)
                .Returns(tenantId);
        }

        private void SetupDatabaseNameProviderMock(string databaseName, string tenantId) 
        {
            databaseNameProviderMock
                .Setup(databaseNameProvider => databaseNameProvider.Get(tenantId))
                .Returns(databaseName);
        }

        private MockRepository mockRepository;
        private Mock<IInvoicesDatabaseNameProvider> databaseNameProviderMock;
        private Mock<IApplicationContext> applicationContextMock;
        private ConnectionStringProvider target;
        private IOptions<DatabaseOptions> optionsAccessor;
        private const string connectionString = "Database Connection String {0} Template";
        private const string tenantId = "tenant";
        private const string databaseName = "Invoices_tenant";
    }
}
