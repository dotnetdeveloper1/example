using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Identity.DataAccess.UnitTests.Database
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class SqlDatabaseContextFactoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            var optionsAccessor = Options.Create(options);
            target = new MasterDatabaseContextFactory(optionsAccessor);
        }

        [TestMethod]
        public void Instance_WhenDatabaseOptionsAccessorIsNull_ShouldThrowArgumentNullException()
        {
            IOptions<SqlManagementClientOptions> options = null;

            Assert.ThrowsException<ArgumentNullException>(() => new MasterDatabaseContextFactory(options));
        }

        [TestMethod]
        public void Instance_WhenDatabaseOptionsAreNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<SqlManagementClientOptions>(null);

            Assert.ThrowsException<ArgumentNullException>(() => new MasterDatabaseContextFactory(optionsAccessor));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenConnectionStringIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string connectionString)
        {
            var options = new SqlManagementClientOptions
            {
                MasterConnectionString = connectionString,
                DefaultDatabaseName = "DefaultDatabaseName",
                CommandTimeoutInSeconds = 90
            };
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() => new MasterDatabaseContextFactory(optionsAccessor));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenDatabaseNameIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string databaseName)
        {
            var options = new SqlManagementClientOptions 
            { 
                MasterConnectionString = "ConnectionString", 
                DefaultDatabaseName = databaseName,
                CommandTimeoutInSeconds = 90
            };
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() => new MasterDatabaseContextFactory(optionsAccessor));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void Instance_WhenCommandTimeoutInSecondsIsZeroOrLess_ShouldThrowArgumentException(int commandTimeoutInSeconds)
        {
            var options = new SqlManagementClientOptions
            {
                MasterConnectionString = "ConnectionString",
                DefaultDatabaseName = "DatabaseName",
                CommandTimeoutInSeconds = commandTimeoutInSeconds
            };

            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentException>(() => new MasterDatabaseContextFactory(optionsAccessor));
        }

        [TestMethod]
        public void Create_ShouldReturnContextWithoutEntityTracking()
        {
            using (var context = target.Create())
            {
                Assert.IsNotNull(context);
                Assert.IsInstanceOfType(context, typeof(IMasterDatabaseContext));

                var dbContext = context as MasterDatabaseContext;

                Assert.IsNotNull(dbContext);
                Assert.AreEqual(QueryTrackingBehavior.NoTracking, dbContext.ChangeTracker.QueryTrackingBehavior);
            }
        }

        private MasterDatabaseContextFactory target;

        private readonly SqlManagementClientOptions options = new SqlManagementClientOptions
        {
            CommandTimeoutInSeconds = 90,
            DefaultDatabaseName = "DefaultDatabaseName",
            MasterConnectionString = "MasterConnectionString"
        };
    }
}
