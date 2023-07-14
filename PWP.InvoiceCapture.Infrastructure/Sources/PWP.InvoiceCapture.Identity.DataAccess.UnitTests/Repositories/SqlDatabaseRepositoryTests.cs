using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.UnitTests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.tenantCapture.tenantManagement.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class SqlDatabaseRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryMasterDatabaseContextFactory();
            cancellationToken = CancellationToken.None;
            context = (MasterDatabaseContext)contextFactory.Create();
            target = new SqlDatabaseRepository(contextFactory, Options.Create<SqlManagementClientOptions>(options));
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod]
        public void Instance_WhenOptionsAccessorAreNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SqlDatabaseRepository(contextFactory, null));
        }

        [TestMethod]
        public void Instance_WhenDbContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SqlDatabaseRepository(null, Options.Create(options)));
        }

        [TestMethod]
        public void Instance_WhenOptionsAreNull_ShouldThrowArgumentNullException()
        {
            var optionsAccessor = Options.Create<SqlManagementClientOptions>(null);

            Assert.ThrowsException<ArgumentNullException>(() => new SqlDatabaseRepository(contextFactory, optionsAccessor));
        }

        [TestMethod]
        [DataRow(-123)]
        [DataRow(0)]
        public void Instance_WhenCommandTimeoutInSecondsIsZeroOrLess_ShouldThrowArgumentException(int commandTimeoutInSeconds)
        {
            options.CommandTimeoutInSeconds = commandTimeoutInSeconds;
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentException>(() => new SqlDatabaseRepository(contextFactory, optionsAccessor));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("    ")]
        public void Instance_WhenDefaultDatabaseNameIsNullOrWhitespace_ShouldThrowArgumentException(string defaultDatabaseName)
        {
            options.DefaultDatabaseName = defaultDatabaseName;
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() => new SqlDatabaseRepository(contextFactory, optionsAccessor));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("    ")]
        public void Instance_WhenMasterConnectionStringIsNullOrWhitespace_ShouldThrowArgumentException(string masterConnectionString)
        {
            options.MasterConnectionString = masterConnectionString;
            var optionsAccessor = Options.Create(options);

            Assert.ThrowsException<ArgumentNullException>(() => new SqlDatabaseRepository(contextFactory, optionsAccessor));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void CreateDatabase_WhenDatabaseNameIsNullOrWhiteSpace_ShouldThrowArgumentException(string databaseName)
        {
             Assert.ThrowsException<ArgumentNullException>(() => target.CreateDatabase(databaseName, cancellationToken));
        }

        [TestMethod]
        [DataRow(10)]
        public async Task GetListAsync_ShouldReturnDatabasesListAsync(int count)
        {
            var databases = Enumerable
                .Range(1, count)
                .Select(index => CreateSqlDatabase())
                .ToList();

            context.Databases.AddRange(databases);
            context.SaveChanges();

            var actualDatabases = await target.GetListAsync(cancellationToken);

            for (int i = 0; i < actualDatabases.Count; i++)
            {
                var expectedDatabase = databases.First(database => database.Name == actualDatabases[i].Name);
                AssertDatabasesAreEqual(expectedDatabase, actualDatabases[i]);
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

        private readonly SqlManagementClientOptions options = new SqlManagementClientOptions()
        {
            CommandTimeoutInSeconds = 90,
            DefaultDatabaseName = "DefaultDatabaseName",
            MasterConnectionString = "MasterConnectionString"
        };

        private MasterDatabaseContext context;
        private IMasterDatabaseContextFactory contextFactory;
        private ISqlDatabaseRepository target;
        private CancellationToken cancellationToken;
    }
}
