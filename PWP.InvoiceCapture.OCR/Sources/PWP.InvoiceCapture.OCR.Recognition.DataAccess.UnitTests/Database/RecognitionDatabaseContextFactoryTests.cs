using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.OCR.Core.DataAccess;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Options;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Database;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class RecognitionDatabaseContextFactoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            var optionsAccessor = Options.Create(options);
            target = new RecognitionDatabaseContextFactory(optionsAccessor);
        }

        [TestMethod]
        public void Instance_WhenRecognitionDatabaseContextOptionsAreNull_ShouldThrowArgumentNullException()
        {
            DbContextOptions<RecognitionDatabaseContext> options = null;

            Assert.ThrowsException<ArgumentNullException>(() => new RecognitionDatabaseContextFactory(options));
        }

        [TestMethod]
        public void Instance_WhenRecognitionDatabaseOptionsAccessorIsNull_ShouldThrowArgumentNullException()
        {
            IOptions<RecognitionDatabaseOptions> options = null;

            Assert.ThrowsException<ArgumentNullException>(() => new RecognitionDatabaseContextFactory(options));
        }

        [TestMethod]
        public void Instance_WhenRecognitionDatabaseOptionsAreNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<RecognitionDatabaseOptions>(null);

            Assert.ThrowsException<ArgumentNullException>(() => new RecognitionDatabaseContextFactory(optionsAccessor));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Instance_WhenConnectionStringIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string connectionString)
        {
            var options = new RecognitionDatabaseOptions { ConnectionString = connectionString };
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() => new RecognitionDatabaseContextFactory(optionsAccessor));
        }

        [TestMethod]
        public void Create_ShouldReturnContextWithoutEntityTracking()
        {
            using (var context = target.Create())
            {
                Assert.IsNotNull(context);
                Assert.IsInstanceOfType(context, typeof(IRecognitionDatabaseContext));

                var dbContext = context as RecognitionDatabaseContext;

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
                Assert.IsInstanceOfType(context, typeof(IRecognitionDatabaseContext));

                var dbContext = context as RecognitionDatabaseContext;

                Assert.IsNotNull(dbContext);
                Assert.AreEqual(QueryTrackingBehavior.TrackAll, dbContext.ChangeTracker.QueryTrackingBehavior);
            }
        }

        private RecognitionDatabaseContextFactory target;

        private readonly RecognitionDatabaseOptions options = new RecognitionDatabaseOptions
        {
            ConnectionString = "fakeConnectionString"
        };
    }
}
