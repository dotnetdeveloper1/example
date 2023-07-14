using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.OCR.Core.DataAccess;
using PWP.InvoiceCapture.OCR.DataAnalysis.DataAccess.Database;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.DataAccess.UnitTests.Database
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class OCRDatabaseContextFactoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            var optionsAccessor = Options.Create(options);
            target = new OCRDatabaseContextFactory(optionsAccessor);
        }

        [TestMethod]
        public void Instance_WhenDatabaseContextOptionsAreNull_ShouldThrowArgumentNullException()
        {
            DbContextOptions<OCRDatabaseContext> options = null;

            Assert.ThrowsException<ArgumentNullException>(() => new OCRDatabaseContextFactory(options));
        }

        [TestMethod]
        public void Instance_WhenDatabaseOptionsAccessorIsNull_ShouldThrowArgumentNullException()
        {
            IOptions<DatabaseOptions> options = null;

            Assert.ThrowsException<ArgumentNullException>(() => new OCRDatabaseContextFactory(options));
        }

        [TestMethod]
        public void Instance_WhenDatabaseOptionsAreNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<DatabaseOptions>(null);

            Assert.ThrowsException<ArgumentNullException>(() => new OCRDatabaseContextFactory(optionsAccessor));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenConnectionStringIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string connectionString)
        {
            var options = new DatabaseOptions { ConnectionString = connectionString };
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() => new OCRDatabaseContextFactory(optionsAccessor));
        }

        [TestMethod]
        public void Create_ShouldReturnContextWithoutEntityTracking()
        {
            using (var context = target.Create())
            {
                Assert.IsNotNull(context);
                Assert.IsInstanceOfType(context, typeof(IDatabaseContext));

                var dbContext = context as OCRDatabaseContext;

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
                Assert.IsInstanceOfType(context, typeof(IDatabaseContext));

                var dbContext = context as OCRDatabaseContext;

                Assert.IsNotNull(dbContext);
                Assert.AreEqual(QueryTrackingBehavior.TrackAll, dbContext.ChangeTracker.QueryTrackingBehavior);
            }
        }

        private OCRDatabaseContextFactory target;

        private readonly DatabaseOptions options = new DatabaseOptions
        {
            ConnectionString = "fakeConnectionString"
        };
    }
}
