using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TenantServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            tenantRepositoryMock = mockRepository.Create<ITenantRepository>();
            tenantSettingsRepositoryMock = mockRepository.Create<ITenantSettingRepository>();
            sqlManagementServiceMock = mockRepository.Create<ISqlManagementService>();
            invoicesDatabaseNameProviderMock = mockRepository.Create<IInvoicesDatabaseNameProvider>();
            emailProviderMock = mockRepository.Create<IEmailProvider>();

            target = new TenantService(tenantRepositoryMock.Object, tenantSettingsRepositoryMock.Object, sqlManagementServiceMock.Object, invoicesDatabaseNameProviderMock.Object, emailProviderMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenTenantRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TenantService(null, tenantSettingsRepositoryMock.Object, sqlManagementServiceMock.Object, invoicesDatabaseNameProviderMock.Object, emailProviderMock.Object));
        }

        [TestMethod]
        public void Instance_WhenSqlManagementServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TenantService(tenantRepositoryMock.Object, tenantSettingsRepositoryMock.Object, null, invoicesDatabaseNameProviderMock.Object, emailProviderMock.Object));
        }

        [TestMethod]
        public void Instance_WhenInvoicesDatabaseNameProviderIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TenantService(tenantRepositoryMock.Object, tenantSettingsRepositoryMock.Object, sqlManagementServiceMock.Object, null, emailProviderMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTenantSettingsRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TenantService(tenantRepositoryMock.Object, null, sqlManagementServiceMock.Object, invoicesDatabaseNameProviderMock.Object, emailProviderMock.Object));
        }

        [TestMethod]
        public void Instance_WhenEmailProviderIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TenantService(tenantRepositoryMock.Object, tenantSettingsRepositoryMock.Object, sqlManagementServiceMock.Object, invoicesDatabaseNameProviderMock.Object, null));
        }

        [TestMethod]
        [DataRow(int.MaxValue)]
        public async Task GetListExceptStatusAsync_WhenStatusIsWrong_ShouldThrowArgumentNullExceptionAsync(int statusValue)
        {
            var status = (TenantDatabaseStatus)statusValue;
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetListExceptStatusAsync(status, cancellationToken));
        }

        [TestMethod]
        public async Task GetListAsync_WhenTenantCollectionExists_ShouldReturnTenants()
        {
            var expectedTenants = new List<Tenant>() { CreateTenant("tenant", 1, TenantDatabaseStatus.Copied, 1) };

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedTenants);

            var actualTenants = await target.GetListAsync(cancellationToken);

            Assert.AreEqual(actualTenants.Count(), 1);
            AssertTenantsAreEqual(actualTenants[0], expectedTenants[0]);
        }

        [TestMethod]
        public async Task GetListExceptStatusAsync_WhenTenantCollectionExists_ShouldReturnTenants()
        {
            var status = TenantDatabaseStatus.Copying;
            var exceptStatus = TenantDatabaseStatus.Copied;

            var expectedTenants = new List<Tenant>() { CreateTenant("tenant", 1, status, 1) };

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetListExceptStatusAsync(exceptStatus, cancellationToken))
                .ReturnsAsync(expectedTenants);

            var actualTenants = await target.GetListExceptStatusAsync(exceptStatus, cancellationToken);

            Assert.AreEqual(actualTenants.Count(), 1);
            AssertTenantsAreEqual(actualTenants[0], expectedTenants[0]);
        }

        [TestMethod]
        public async Task GetListAsync_WhenTenantRepositoryThrowError_ShouldThrowTimeoutException()
        {
            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetListAsync(cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetListAsync(cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-123)]
        public async Task GetAsync_WhenTenantIdIsZeroOrNegative_ShouldThrowArgumentException(int tenantId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.GetAsync(tenantId, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenTenantNotFound_ShouldReturnNull(int tenantId)
        {
            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync((Tenant)null);

            var actualTenant = await target.GetAsync(tenantId, cancellationToken);

            Assert.IsNull(actualTenant);
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, 10)]
        public async Task GetAsync_WhenTenantFound_ShouldReturnTenant(int tenantId, int groupId)
        {
            var expectedTenant = CreateTenant("tenant name", tenantId, TenantDatabaseStatus.Copied, groupId);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(expectedTenant);

            var actualTenant = await target.GetAsync(tenantId, cancellationToken);

            AssertTenantsAreEqual(actualTenant, expectedTenant);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task GetAsync_WhenTenantRepositoryThrowError_ShouldThrowTimeoutException(int tenantId)
        {
            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetAsync(tenantId, cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetAsync(tenantId, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenTenantViewModelIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(null, cancellationToken));
        }
        
        [TestMethod]
        [DataRow(null, 1)]
        [DataRow("", 1)]
        [DataRow("    ", 1)]
        public async Task CreateAsync_WhenTenantNameIsNullOrWhitespace_ShouldReturnFailedOperation(string tenantName, int groupId)
        {
            var tenantCreationParameters = CreateTenantViewModel(tenantName, groupId);

            var actualResult = await target.CreateAsync(tenantCreationParameters, cancellationToken);

            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Failed);
            Assert.AreEqual(actualResult.Message, "Tenant name required.");
        }

        [TestMethod]
        [DataRow("tenant name 1", 0)]
        [DataRow("tenant name 1", -1)]
        public async Task CreateAsync_WhenGroupIdIsZeroOrNegative_ShouldReturnFailedOperation(string tenantName, int groupId)
        {
            var tenantCreationParameters = CreateTenantViewModel(tenantName, groupId);

            var actualResult = await target.CreateAsync(tenantCreationParameters, cancellationToken);

            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Failed);
            Assert.AreEqual(actualResult.Message, "GroupId must be greater than zero.");
        }

        [TestMethod]
        [DataRow("tenant name 1", 1)]
        [DataRow("tenant name 2", 2)]
        public async Task CreateAsync_WhenTenantWithNameAlreadyExists_ShouldReturnFailedOperation(string tenantName, int groupId)
        {
            var tenantCreationParameters = CreateTenantViewModel(tenantName, groupId);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.LockAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.LockAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.TenantNameExistsInGroupAsync(tenantName, groupId, cancellationToken))
                .ReturnsAsync(true);

            emailProviderMock
                .Setup(emailProvider => emailProvider.Generate())
                .Returns(email);

            var actualResult = await target.CreateAsync(tenantCreationParameters, cancellationToken);

            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Failed);
            Assert.AreEqual(actualResult.Message, $"Tenant with name '{tenantName}' already exists.");
        }

        [TestMethod]
        [DataRow("tenant name 1", 1, 1, "db 1")]
        [DataRow("tenant name 2", 2, 2, "db 2")]
        public async Task CreateAsync_WhenDatabaseWasCreated_ShouldReturnSuccessOperation(string tenantName, int tenantId, int groupId, string dbName)
        {
            var tenantCreationParameters = CreateTenantViewModel(tenantName, groupId);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.LockAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.TenantNameExistsInGroupAsync(tenantName, groupId, cancellationToken))
                .ReturnsAsync(false);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.CreateAsync(It.IsAny<Tenant>(), cancellationToken))
                .ReturnsAsync(tenantId);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.UpdateAsync(It.IsAny<Tenant>(), cancellationToken))
                .Returns(Task.CompletedTask);

            sqlManagementServiceMock
                .Setup(sqlManagementService => sqlManagementService.CreateSqlDatabase(It.IsAny<string>(), cancellationToken))
                .Returns(TenantDatabaseStatus.Copying);

            invoicesDatabaseNameProviderMock
                .Setup(invoicesDatabaseNameProvider => invoicesDatabaseNameProvider.Get(It.IsAny<string>()))
                .Returns(dbName);

            emailProviderMock
                .Setup(emailProvider => emailProvider.Generate())
                .Returns(email);

            var actualResult = await target.CreateAsync(tenantCreationParameters, cancellationToken);

            Assert.IsTrue(actualResult.IsSuccessful);
            Assert.IsNotNull(actualResult.Data);
            Assert.AreEqual(tenantId, actualResult.Data.Id);
            Assert.AreEqual(dbName, actualResult.Data.DatabaseName);
            Assert.AreEqual(groupId, actualResult.Data.GroupId);
            Assert.AreEqual(tenantName, actualResult.Data.Name);
            Assert.AreEqual(OperationResultStatus.Success,actualResult.Status);
            Assert.AreEqual($"Tenant with name '{tenantCreationParameters.TenantName}' is creating.", actualResult.Message );
        }


        [TestMethod]
        public async Task CheckTenantsDatabasesStateAsync_WhenNoTenantsToUpdate_ShouldNotThrowException()
        {
            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(new List<Tenant>());

            sqlManagementServiceMock
                .Setup(sqlManagementService => sqlManagementService.GetSqlDatabasesAsync(cancellationToken))
                .ReturnsAsync(new List<SqlDatabase>() { CreateSqlDatabase("db", SqlDatabaseState.Online) });

            await target.CheckTenantsDatabasesStateAsync(cancellationToken);
        }

        [TestMethod]
        [DataRow("database 1")]
        [DataRow("database 2")]
        public async Task CheckTenantsDatabasesStateAsync_WhenWeHaveTenantsToUpdate_ShouldNotThrowException(string databaseName)
        {
            var tenants = new List<Tenant>() { CreateTenant(databaseName) };

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(tenants);

            sqlManagementServiceMock
                .Setup(sqlManagementService => sqlManagementService.GetSqlDatabasesAsync(cancellationToken))
                .ReturnsAsync(new List<SqlDatabase>() { CreateSqlDatabase(databaseName, SqlDatabaseState.Online) });

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.UpdateAsync(tenants, cancellationToken))
                .Returns(Task.CompletedTask);

            sqlManagementServiceMock
                .Setup(sqlManagementService => sqlManagementService.SetupBackupLongTermPolicesAsync(databaseName, cancellationToken))
                .Returns(Task.CompletedTask);

            await target.CheckTenantsDatabasesStateAsync(cancellationToken);
        }

        [TestMethod]
        [DataRow(1, 1, "tenant name 1")]
        [DataRow(2, 2, "tenant name 2")]
        public async Task UpdateTenantNameAsync_WhenTenantDoesntExist(int tenantId, int groupId, string tenantName)
        {
            var tenant = CreateTenant("old tenant name", tenantId, TenantDatabaseStatus.NotCopied, groupId);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.LockAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync((Tenant)null);

            var actualResult = await target.UpdateTenantNameAsync(tenantId, tenantName, cancellationToken);

            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.NotFound);
            Assert.AreEqual(actualResult.Message, $"Tenant with id = {tenantId} doesn't exist.");
        }

        [TestMethod]
        [DataRow(1, 1, "tenant name 1")]
        [DataRow(2, 2, "tenant name 2")]
        public async Task UpdateTenantNameAsync_WhenTenantExistsAndNewTenantNameAlreadyExistsInGroup(int tenantId, int groupId, string tenantName)
        {
            var tenant = CreateTenant("old tenant name", tenantId, TenantDatabaseStatus.NotCopied, groupId);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.LockAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(tenant);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.TenantNameExistsInGroupAsync(tenantName, tenantId, cancellationToken))
                .ReturnsAsync(true);

            var actualResult = await target.UpdateTenantNameAsync(tenantId, tenantName, cancellationToken);

            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Failed);
            Assert.AreEqual(actualResult.Message, $"Tenant with name '{tenantName}' already exists.");
        }

        [TestMethod]
        [DataRow(1, 1, "tenant name 1")]
        [DataRow(2, 2, "tenant name 2")]
        public async Task UpdateTenantNameAsync_WhenTenantExistsAndNewTenantNameDoesntExistInGroup(int tenantId, int groupId, string tenantName)
        {
            var tenant = CreateTenant("old tenant name", tenantId, TenantDatabaseStatus.NotCopied, groupId);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.LockAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(tenant);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.TenantNameExistsInGroupAsync(tenantName, tenantId, cancellationToken))
                .ReturnsAsync(false);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.UpdateAsync(tenant, cancellationToken))
                .Returns(Task.CompletedTask);

            var actualResult = await target.UpdateTenantNameAsync(tenantId, tenantName, cancellationToken);

            Assert.IsTrue(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Success);
            Assert.AreEqual(actualResult.Message, $"Tenant name with id = {tenantId} updated successfuly");
        }

        [TestMethod]
        [DataRow(1, "newName")]
        public async Task CloneAsync_ShouldCloneAsync(int tenantId, string newTenantName)
        {
            CreateMocksForTenantsCloning(tenantId, newTenantName);

            var settings = new TenantSetting { TenantId = newTenantId, CultureId = cultureId };

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.LockAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            tenantSettingsRepositoryMock
                .Setup(tenantSettingsRepository => tenantSettingsRepository.GetByTenantIdAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(settings);

            tenantSettingsRepositoryMock
                .Setup(tenantSettingsRepository => tenantSettingsRepository.CreateAsync(It.IsAny<TenantSetting>(), cancellationToken))
                .ReturnsAsync(It.IsAny<int>());

            emailProviderMock
                .Setup(emailProvider => emailProvider.Generate())
                .Returns(email);


            var actualResult = await target.CloneAsync(tenantId, newTenantName, cancellationToken);

            Assert.IsTrue(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Success);
            Assert.AreEqual(actualResult.Message, $"Tenant with name '{newTenantName}' is creating.");
        }

        [TestMethod]
        [DataRow(1, "newName")]
        public async Task CloneAsync_WhenTennantSettingsAreNotExist_ShouldCloneAsync(int tenantId, string newTenantName)
        {
            CreateMocksForTenantsCloning(tenantId, newTenantName);

            var settings = new TenantSetting { TenantId = newTenantId, CultureId = cultureId };

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.LockAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            tenantSettingsRepositoryMock
                .Setup(tenantSettingsRepository => tenantSettingsRepository.GetByTenantIdAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync((TenantSetting)null);

            emailProviderMock
                .Setup(emailProvider => emailProvider.Generate())
                .Returns(email);

            var actualResult = await target.CloneAsync(tenantId, newTenantName, cancellationToken);

            Assert.IsTrue(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Success);
            Assert.AreEqual(actualResult.Message, $"Tenant with name '{newTenantName}' is creating.");

        }

        [TestMethod]
        [DataRow(1, "newName")]
        public async Task CloneAsync_WhenTennantIsNotExists_ShouldReturnFailedResultAsync(int tenantId, string newTenantName)
        {
            var userCreationResponse = CreateUserCreationResponse(true);
            tenantRepositoryMock
               .Setup(tenantRepository => tenantRepository.GetAsync(tenantId, cancellationToken))
               .ReturnsAsync((Tenant)null);


            var actualResult = await target.CloneAsync(tenantId, newTenantName, cancellationToken);
            
            Assert.IsNotNull(actualResult);
            Assert.IsFalse(actualResult.IsSuccessful);
            Assert.AreEqual(actualResult.Status, OperationResultStatus.Failed);
            Assert.AreEqual(actualResult.Message, $"There is no tenant with id = {tenantId}.");
            
        }

        [DataRow(0)]
        [DataRow(-1)]
        [TestMethod]
        public async Task CloneAsync_WhenTenantIdIsWrong_ShouldReturnFailedOperationResult(int tenantIds)
        {
            var result = await target.CloneAsync(tenantIds, "someName", cancellationToken);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(OperationResultStatus.Failed, result.Status);
            Assert.AreEqual("Tenant id required.", result.Message);
        }
      
        [TestMethod]
        public async Task GetTenantIdByEmailAsync_WhenTenantNotExists_ShouldReturn0Async()
        {
            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetIdByUploadEmailAsync(uploadEmail, cancellationToken))
                .ReturnsAsync((Tenant)null);

            var actualResult = await target.GetTenantIdByEmailAsync(uploadEmail, cancellationToken);

            Assert.IsNull(actualResult);
        }

        [TestMethod]
        public async Task GetTenantIdByEmailAsync_WhenTenantExists_ShouldReturnTenantIdAsync()
        {
            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.GetIdByUploadEmailAsync(uploadEmail, cancellationToken))
                .ReturnsAsync(new Tenant() { Id = tenantId });

            var actualResult = await target.GetTenantIdByEmailAsync(uploadEmail, cancellationToken);

            Assert.AreEqual(tenantId.ToString(), actualResult);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public async Task GetTenantIdByEmailAsync_WhenUploadEmailIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string uploadEmail)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.GetTenantIdByEmailAsync(uploadEmail, cancellationToken));
        }

        private void CreateMocksForTenantsCloning(int tenantId, string newTenantName)
        {
            var newTenant = CreateTenant(newTenantName, newTenantId, TenantDatabaseStatus.NotCopied, groupId);
            var userCreationResponse = CreateUserCreationResponse(true);
            tenantRepositoryMock
               .Setup(tenantRepository => tenantRepository.GetAsync(tenantId, cancellationToken))
               .ReturnsAsync(new Tenant() { GroupId = groupId });

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.TenantNameExistsInGroupAsync(newTenantName, groupId, cancellationToken))
                .ReturnsAsync(false);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.CreateAsync(It.IsAny<Tenant>(), cancellationToken))
                .ReturnsAsync(tenantId);

            tenantRepositoryMock
                .Setup(tenantRepository => tenantRepository.UpdateAsync(It.IsAny<Tenant>(), cancellationToken))
                .Returns(Task.CompletedTask);

            sqlManagementServiceMock
                .Setup(sqlManagementService => sqlManagementService.CreateSqlDatabase(It.IsAny<string>(), cancellationToken))
                .Returns(TenantDatabaseStatus.Copying);

            invoicesDatabaseNameProviderMock
                .Setup(invoicesDatabaseNameProvider => invoicesDatabaseNameProvider.Get(It.IsAny<string>()))
                .Returns("newDb");

        }

        private void AssertTenantsAreEqual(Tenant actual, Tenant expected)
        {
            Assert.AreEqual(expected.DatabaseName, actual.DatabaseName);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            Assert.AreEqual(expected.IsActive, actual.IsActive);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.Id, actual.Id);
        }

        private Tenant CreateTenant(string tenantName, int tenantId, TenantDatabaseStatus status, int groupId)
        {
            return new Tenant()
            {
                Id = tenantId,
                DatabaseName = "DatabaseName",
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Name = tenantName,
                Status = status,
                GroupId = groupId
            };
        }

        private OperationResult<UserCreationResponse> CreateUserCreationResponse(bool isSuccess)
        {
            return new OperationResult<UserCreationResponse>
            {
                Data = isSuccess ? new UserCreationResponse() { Id = 1, Username = "username", Password = "password" } : null,
                Status = isSuccess ? OperationResultStatus.Success : OperationResultStatus.Failed,
                Message = "Message"
            };
        }

        private TenantCreationParameters CreateTenantViewModel(string tenantName, int groupId)
        {
            return new TenantCreationParameters()
            {
                TenantName = tenantName,
                GroupId = groupId
            };
        }

        private Tenant CreateTenant(string databaseName)
        {
            return new Tenant
            {
                DatabaseName = databaseName,
                Status = TenantDatabaseStatus.Copying
            };
        }

        private SqlDatabase CreateSqlDatabase(string name, SqlDatabaseState state)
        {
            return new SqlDatabase
            {
                Name = name,
                State = state,
                StateDescription = state.ToString()
            };
        }

        private MockRepository mockRepository;
        private Mock<ITenantRepository> tenantRepositoryMock;
        private Mock<ITenantSettingRepository> tenantSettingsRepositoryMock;
        private Mock<ISqlManagementService> sqlManagementServiceMock;
        private Mock<IInvoicesDatabaseNameProvider> invoicesDatabaseNameProviderMock;
        private Mock<IEmailProvider> emailProviderMock;
        private TenantService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int groupId = 111;
        private const int newTenantId = 222;
        private const int cultureId = 33;
        private const string email = "someTestEmail";
        private const string uploadEmail = "someUploadEmail@com";
        private const int tenantId = 145;
    }
}
