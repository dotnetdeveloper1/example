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
    public class TenantsDatabaseContextFactoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            var optionsAccessor = Options.Create(options);
            target = new TenantsDatabaseContextFactory(optionsAccessor);
        }

        [TestMethod]
        public void Instance_WhenDatabaseContextOptionsAreNull_ShouldThrowArgumentNullException()
        {
            DbContextOptions<TenantsDatabaseContext> options = null;

            Assert.ThrowsException<ArgumentNullException>(() => new TenantsDatabaseContextFactory(options));
        }

        [TestMethod]
        public void Instance_WhenDatabaseOptionsAccessorIsNull_ShouldThrowArgumentNullException()
        {
            IOptions<DatabaseOptions> options = null;

            Assert.ThrowsException<ArgumentNullException>(() => new TenantsDatabaseContextFactory(options));
        }

        [TestMethod]
        public void Instance_WhenDatabaseOptionsAreNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<DatabaseOptions>(null);

            Assert.ThrowsException<ArgumentNullException>(() => new TenantsDatabaseContextFactory(optionsAccessor));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenConnectionStringIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string connectionString)
        {
            var options = new DatabaseOptions { ConnectionString = connectionString };
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() => new TenantsDatabaseContextFactory(optionsAccessor));
        }

        [TestMethod]
        public void Create_ShouldReturnContextWithoutEntityTracking()
        {
            using (var context = target.Create())
            {
                Assert.IsNotNull(context);
                Assert.IsInstanceOfType(context, typeof(ITenantsDatabaseContext));

                var dbContext = context as TenantsDatabaseContext;

                Assert.IsNotNull(dbContext);
                Assert.AreEqual(QueryTrackingBehavior.NoTracking, dbContext.ChangeTracker.QueryTrackingBehavior);
            }
        }

        [TestMethod]
        public void CreateWithTracking_ShouldReturnContextWithEntityTracking()
        {
            using (var context = target.CreateWithTracking())
            {
                Assert.IsNotNull(context);
                Assert.IsInstanceOfType(context, typeof(ITenantsDatabaseContext));

                var dbContext = context as TenantsDatabaseContext;

                Assert.IsNotNull(dbContext);
                Assert.AreEqual(QueryTrackingBehavior.TrackAll, dbContext.ChangeTracker.QueryTrackingBehavior);
            }
        }

        private TenantsDatabaseContextFactory target;

        private readonly DatabaseOptions options = new DatabaseOptions
        {
            ConnectionString = "fakeConnectionString"
        };
    }
}
