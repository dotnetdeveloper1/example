using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SqlManagementServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            sqlDatabaseRepositoryMock = mockRepository.Create<ISqlDatabaseRepository>();
            target = new SqlManagementService(sqlDatabaseRepositoryMock.Object, Options.Create(options));
        }

        [TestMethod]
        public void Instance_WhenSqlDatabaseRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SqlManagementService(null, Options.Create(options)));
        }

        [TestMethod]
        public void Instance_WhenOptionsAccessorAreNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SqlManagementService(sqlDatabaseRepositoryMock.Object, null));
        }

        [TestMethod]
        public void Instance_WhenOptionsAreNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<LongTermSqlServerBackupOptions>(null);

            Assert.ThrowsException<ArgumentNullException>(() => new SqlManagementService(sqlDatabaseRepositoryMock.Object, optionsAccessor));
        }

        [TestMethod]
        [DataRow("    ")]
        [DataRow("")]
        [DataRow(null)]
        public void CreateSqlDatabase_WhenDatabaseNameIsNullOrWhitespace_ShouldReturnArgumentNullException(string databaseName)
        {
            Assert.ThrowsException<ArgumentNullException>(() => target.CreateSqlDatabase(databaseName, cancellationToken));
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(10)]
        public async Task GetListAsync_ShouldDatabases(int count)
        {
            var databases = Enumerable
                .Range(1, count)
                .Select(index => CreateSqlDatabase())
                .ToList();

            sqlDatabaseRepositoryMock
                .Setup(sqlDatabaseRepository => sqlDatabaseRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(databases);

            var actualDatabases = await target.GetSqlDatabasesAsync(cancellationToken);

            for (int i = 0; i < actualDatabases.Count; i++)
            {
                AssertDatabasesAreEqual(databases[i], actualDatabases[i]);
            }
        }

        private SqlDatabase CreateSqlDatabase()
        {
            return new SqlDatabase
            {
                Name = Guid.NewGuid().ToString(),
                State = SqlDatabaseState.Online,
                StateDescription = "StateDescription"
            };
        }

        private void AssertDatabasesAreEqual(SqlDatabase expected, SqlDatabase actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.State, actual.State);
            Assert.AreEqual(expected.StateDescription, actual.StateDescription);

            // Ensure all properties are tested
            Assert.AreEqual(3, actual.GetType().GetProperties().Length);
        }

        private readonly LongTermSqlServerBackupOptions options = new LongTermSqlServerBackupOptions()
        {
            TenantId = "tenant id",
            SqlServerName = "server name",
            SubscriptionId = "some id",
            Enabled = true,
            ResourceGroupName = "resourse group name",
            WeeklyRetention = "1w",
            MonthlyRetention = "2m",
            YearlyRetention = "1y",
            WeekOfYear = 1
        };

        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private SqlManagementService target;
        private MockRepository mockRepository;
        private Mock<ISqlDatabaseRepository> sqlDatabaseRepositoryMock;
    }
}
