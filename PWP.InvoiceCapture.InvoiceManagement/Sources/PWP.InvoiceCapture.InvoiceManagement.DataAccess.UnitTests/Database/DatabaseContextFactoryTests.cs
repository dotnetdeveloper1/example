using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.UnitTests.Database
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DatabaseContextFactoryTests
    {
        [TestInitialize]
        public void Initialize() 
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            connectionStringProviderMock = mockRepository.Create<IConnectionStringProvider>();

            target = new DatabaseContextFactory(connectionStringProviderMock.Object);
        }

        [TestMethod]
        public void Instance_WhenDatabaseContextOptionsAreNull_ShouldThrowArgumentNullException()
        {
            DbContextOptions<DatabaseContext> options = null;

            Assert.ThrowsException<ArgumentNullException>(() => new DatabaseContextFactory(options));
        }

        [TestMethod]
        public void Instance_WhenConnectionStringProviderIsNull_ShouldThrowArgumentNullException() 
        {
            IConnectionStringProvider connectionStringProvider = null;

            Assert.ThrowsException<ArgumentNullException>(() => new DatabaseContextFactory(connectionStringProvider));
        }

        [TestMethod]
        public void Create_ShouldReturnContextWithoutEntityTracking() 
        {
            SetupConnectionStringProviderMock();

            using (var context = target.Create())
            {
                Assert.IsNotNull(context);
                Assert.IsInstanceOfType(context, typeof(IDatabaseContext));

                var dbContext = context as DatabaseContext;

                Assert.IsNotNull(dbContext);
                Assert.AreEqual(QueryTrackingBehavior.NoTracking, dbContext.ChangeTracker.QueryTrackingBehavior);
            }
        }

        [TestMethod]
        public void CreateWithTracking_ShouldReturnContextWithEntityTracking()
        {
            SetupConnectionStringProviderMock();

            using (var context = target.CreateWithTracking())
            {
                Assert.IsNotNull(context);
                Assert.IsInstanceOfType(context, typeof(IDatabaseContext));

                var dbContext = context as DatabaseContext;

                Assert.IsNotNull(dbContext);
                Assert.AreEqual(QueryTrackingBehavior.TrackAll, dbContext.ChangeTracker.QueryTrackingBehavior);
            }
        }

        private void SetupConnectionStringProviderMock() 
        {
            connectionStringProviderMock
                .Setup(connectionStringProvider => connectionStringProvider.Get())
                .Returns(connectionString);
        }

        private MockRepository mockRepository;
        private Mock<IConnectionStringProvider> connectionStringProviderMock;
        private DatabaseContextFactory target;
        private const string connectionString = "fakeConnectionString";
    }
}
