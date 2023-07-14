using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Identity.Business.Contract.Definitions;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ResourceOwnerPasswordValidatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            passwordHashServiceMock = mockRepository.Create<IPasswordHashService>();
            userServiceMock = mockRepository.Create<IUserService>();
            tenantServiceMock = mockRepository.Create<ITenantService>();
            tenantSettingServiceMock = mockRepository.Create<ITenantSettingService>();
            cultureServiceMock = mockRepository.Create<ICultureService>();

            target = new ResourceOwnerPasswordValidator(passwordHashServiceMock.Object,
                userServiceMock.Object,
                tenantServiceMock.Object,
                tenantSettingServiceMock.Object,
                cultureServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenPasswordHashServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResourceOwnerPasswordValidator(null,
                userServiceMock.Object,
                tenantServiceMock.Object,
                tenantSettingServiceMock.Object,
                cultureServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenUserServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResourceOwnerPasswordValidator(passwordHashServiceMock.Object,
                null,
                tenantServiceMock.Object,
                tenantSettingServiceMock.Object,
                cultureServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTenantServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResourceOwnerPasswordValidator(passwordHashServiceMock.Object,
                userServiceMock.Object,
                null,
                tenantSettingServiceMock.Object,
                cultureServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenTenantSettingServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResourceOwnerPasswordValidator(passwordHashServiceMock.Object,
                userServiceMock.Object,
                tenantServiceMock.Object,
                null,
                cultureServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenCultureServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ResourceOwnerPasswordValidator(passwordHashServiceMock.Object,
                userServiceMock.Object,
                tenantServiceMock.Object,
                tenantSettingServiceMock.Object,
                null));
        }

        [TestMethod]
        [DataRow("   ")]
        [DataRow("")]
        [DataRow(null)]
        public async Task ValidateAsync_WhenUserNameIsNull_ShouldReturnErrorContext(string userName)
        {
            var context = new ResourceOwnerPasswordValidationContext() { UserName = userName, Password = "password" };

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsTrue(context.Result.IsError);
        }

        [TestMethod]
        [DataRow("   ")]
        [DataRow("")]
        [DataRow(null)]
        public async Task ValidateAsync_WhenPasswordIsNull_ShouldReturnErrorContext(string password)
        {
            var context = new ResourceOwnerPasswordValidationContext() { UserName = "username", Password = password };

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsTrue(context.Result.IsError);
        }

        [TestMethod]
        [DataRow("userName_1")]
        [DataRow("userName_2")]
        public async Task ValidateAsync_WhenUserNotExists_ShouldReturnErrorContext(string userName)
        {
            userServiceMock
                .Setup(userService => userService.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync((User)null);

            var context = new ResourceOwnerPasswordValidationContext() { UserName = userName, Password = "password" };

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsTrue(context.Result.IsError);
        }        

        [TestMethod]
        [DataRow("userName_1", "passwordHash")]
        [DataRow("userName_2", "passwordHash")]
        public async Task ValidateAsync_WhenUserPasswordIsWrong_ShouldReturnErrorContext(string userName, string passwordHash)
        {
            var contexts = new List<ResourceOwnerPasswordValidationContext>();

            var user = CreateUser(userName, 1, passwordHash);

            userServiceMock
                .Setup(userService => userService.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(user);

            passwordHashServiceMock
                .Setup(passwordHashService => passwordHashService.GetHash(It.IsAny<string>()))
                .Returns("wrongPasswordHash");

            var context = new ResourceOwnerPasswordValidationContext() { UserName = userName, Password = "password" };

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsTrue(context.Result.IsError);
        }

        [TestMethod]
        [DataRow("userName_1", "passwordHash", 1)]
        [DataRow("userName_2", "passwordHash", 2)]
        public async Task ValidateAsync_WhenRequestIsNull_ShouldReturnContext(string userName, string passwordHash, int tenantId)
        {
            var contexts = new List<ResourceOwnerPasswordValidationContext>();
            var user = CreateUser(userName, tenantId, passwordHash);

            userServiceMock
                .Setup(userService => userService.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(user);

            passwordHashServiceMock
                .Setup(passwordHashService => passwordHashService.GetHash(It.IsAny<string>()))
                .Returns(passwordHash);

            var context = new ResourceOwnerPasswordValidationContext() { UserName = userName, Password = "password" };

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsTrue(context.Result.IsError);
            Assert.IsTrue(string.Equals(context.Result.Error, invalidGrant));
        }

        [TestMethod]
        [DataRow("userName_1", "passwordHash", 1, tenantIdKeyName)]
        [DataRow("userName_2", "passwordHash", 2, tenantIdKeyName)]
        [DataRow("userName_2", "passwordHash", 2, "tenantid")]
        [DataRow("userName_2", "passwordHash", 2, "Tenantid")]
        [DataRow("userName_2", "passwordHash", 2, "TenantId")]
        public async Task ValidateAsync_WhenNoTenantIdInRequest_ShouldReturnContext(string userName, string passwordHash, int tenantId, string tenantIdKeyName)
        {
            var contexts = new List<ResourceOwnerPasswordValidationContext>();
            var user = CreateUser(userName, tenantId, passwordHash);

            userServiceMock
                .Setup(userService => userService.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(user);

            passwordHashServiceMock
                .Setup(passwordHashService => passwordHashService.GetHash(It.IsAny<string>()))
                .Returns(passwordHash);

            var context = CreateValidationContextWithRequest(userName, tenantId.ToString(), tenantIdKeyName);
            context.Request.Raw = new NameValueCollection();

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsTrue(context.Result.IsError);
            Assert.IsTrue(string.Equals(context.Result.Error, invalidGrant));
        }

        [TestMethod]
        [DataRow("userName_1", "passwordHash", 1)]
        [DataRow("userName_2", "passwordHash", 2)]
        public async Task ValidateAsync_WithTenantIsNull_ShouldReturnContext(string userName, string passwordHash, int tenantId)
        {
            var contexts = new List<ResourceOwnerPasswordValidationContext>();
            var user = CreateUser(userName, tenantId, passwordHash);

            userServiceMock
                .Setup(userService => userService.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(user);

            passwordHashServiceMock
                .Setup(passwordHashService => passwordHashService.GetHash(It.IsAny<string>()))
                .Returns(passwordHash);

            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync((Tenant)null);

            var context = CreateValidationContextWithRequest(userName, tenantId.ToString());

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsTrue(context.Result.IsError);
            Assert.IsTrue(string.Equals(context.Result.Error, invalidGrant));
        }

        [TestMethod]
        [DataRow("userName_1", "passwordHash", 1, 1)]
        [DataRow("userName_2", "passwordHash", 2, 2)]
        public async Task ValidateAsync_WithDifferentGroupsRequest_ShouldReturnContext(string userName, string passwordHash, int tenantId, int groupId)
        {
            var contexts = new List<ResourceOwnerPasswordValidationContext>();
            var user = CreateUser(userName, tenantId, passwordHash);
            user.GroupId = groupId;

            userServiceMock
                .Setup(userService => userService.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(user);

            passwordHashServiceMock
                .Setup(passwordHashService => passwordHashService.GetHash(It.IsAny<string>()))
                .Returns(passwordHash);

            var tenant = CreateTenant("databaseName", 1, tenantId);
            tenant.GroupId = groupId + 1;

            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(tenant);

            var context = CreateValidationContextWithRequest(userName, tenantId.ToString());

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsTrue(context.Result.IsError);
            Assert.IsTrue(string.Equals(context.Result.Error, invalidGrant));
        }

        [TestMethod]
        [DataRow("userName_1", "passwordHash", 1, 1)]
        [DataRow("userName_2", "passwordHash", 2, 2)]
        public async Task ValidateAsync_WhenTenantSettingIsNull_ShouldReturnContext(string userName, string passwordHash, int tenantId, int groupId)
        {
            var contexts = new List<ResourceOwnerPasswordValidationContext>();

            var user = CreateUser(userName, tenantId, passwordHash);
            user.GroupId = groupId;

            var tenant = CreateTenant("databaseName", 1, tenantId);
            tenant.GroupId = groupId;

            userServiceMock
                .Setup(userService => userService.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(user);

            passwordHashServiceMock
                .Setup(passwordHashService => passwordHashService.GetHash(It.IsAny<string>()))
                .Returns(passwordHash);

            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(tenant);

            tenantSettingServiceMock
                .Setup(tenantSettingService => tenantSettingService.GetAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync((TenantSetting)null);

            var context = CreateValidationContextWithRequest(userName, tenantId.ToString());

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsFalse(context.Result.IsError);
            Assert.IsTrue(context.Result.Subject.Claims.Any(claim => claim.Type == InvoiceManagementClaims.TenantId && claim.Value == tenantId.ToString()));
            Assert.IsTrue(context.Result.Subject.Claims.Any(claim => claim.Type == InvoiceManagementClaims.Culture && claim.Value == "en-US"));
        }

        [TestMethod]
        [DataRow("userName_1", "passwordHash", 1, 1)]
        [DataRow("userName_2", "passwordHash", 2, 2)]
        public async Task ValidateAsync_WhenCultureIsNull_ShouldReturnContext(string userName, string passwordHash, int tenantId, int groupId)
        {
            var contexts = new List<ResourceOwnerPasswordValidationContext>();

            var user = CreateUser(userName, groupId, passwordHash);
            user.GroupId = groupId;

            var tenant = CreateTenant("databaseName", 1, tenantId);
            tenant.GroupId = groupId;

            var tenantSetting = new TenantSetting { Id = 1, TenantId = tenantId };

            userServiceMock
                .Setup(userService => userService.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(user);

            passwordHashServiceMock
                .Setup(passwordHashService => passwordHashService.GetHash(It.IsAny<string>()))
                .Returns(passwordHash);

            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(tenantId, cancellationToken))
                .ReturnsAsync(tenant);

            tenantSettingServiceMock
                .Setup(tenantSettingService => tenantSettingService.GetAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(tenantSetting);

            cultureServiceMock
                .Setup(cultureService => cultureService.GetAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync((Culture)null);

            var context = CreateValidationContextWithRequest(userName, tenantId.ToString());

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsFalse(context.Result.IsError);
            Assert.IsTrue(context.Result.Subject.Claims.Any(claim => claim.Type == InvoiceManagementClaims.TenantId && claim.Value == tenantId.ToString()));
            Assert.IsTrue(context.Result.Subject.Claims.Any(claim => claim.Type == InvoiceManagementClaims.Culture && claim.Value == "en-US"));
        }

        [TestMethod]
        [DataRow("userName_1", "passwordHash", 1, 1)]
        [DataRow("userName_2", "passwordHash", 2, 2)]
        public async Task ValidateAsync_WhenItIsNotDeveloperDatabase_ShouldReturnContext(string userName, string passwordHash, int tenantId, int groupId)
        {
            var contexts = new List<ResourceOwnerPasswordValidationContext>();

            var user = CreateUser(userName, tenantId, passwordHash);
            user.GroupId = groupId;

            var tenant = CreateTenant("databaseName", 1, tenantId);
            tenant.GroupId = groupId;

            var tenantSetting = new TenantSetting { Id = 1, TenantId = tenantId };

            var culture = new Culture { Id = 1, Name = "en-US" };

            userServiceMock
                .Setup(userService => userService.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(user);

            passwordHashServiceMock
                .Setup(passwordHashService => passwordHashService.GetHash(It.IsAny<string>()))
                .Returns(passwordHash);

            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(user.GroupId, cancellationToken))
                .ReturnsAsync(tenant);

            tenantSettingServiceMock
                .Setup(tenantSettingService => tenantSettingService.GetAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(tenantSetting);

            cultureServiceMock
                .Setup(cultureService => cultureService.GetAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(culture);

            var context = CreateValidationContextWithRequest(userName, tenantId.ToString());

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsFalse(context.Result.IsError);
            Assert.IsTrue(context.Result.Subject.Claims.Any(claim => claim.Type == InvoiceManagementClaims.TenantId && claim.Value == tenantId.ToString()));
            Assert.IsTrue(context.Result.Subject.Claims.Any(claim => claim.Type == InvoiceManagementClaims.Culture && claim.Value == culture.Name));
        }

        [TestMethod]
        [DataRow("userName_1", "passwordHash", 1, 1)]
        [DataRow("userName_2", "passwordHash", 2, 2)]
        public async Task ValidateAsync_WhenItIsDeveloperDatabase_ShouldReturnContext(string userName, string passwordHash, int tenantId, int groupId)
        {
            var contexts = new List<ResourceOwnerPasswordValidationContext>();

            var user = CreateUser(userName, tenantId, passwordHash);
            user.GroupId = groupId;

            var tenant = CreateTenant(developerDatabaseName, 1, tenantId);
            tenant.GroupId = groupId;

            var tenantSetting = new TenantSetting { Id = 1, TenantId = tenantId };
            var culture = new Culture { Id = 1, Name = "en-US" };

            userServiceMock
                .Setup(userService => userService.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(user);

            passwordHashServiceMock
                .Setup(passwordHashService => passwordHashService.GetHash(It.IsAny<string>()))
                .Returns(passwordHash);

            tenantServiceMock
                .Setup(tenantService => tenantService.GetAsync(user.GroupId, cancellationToken))
                .ReturnsAsync(tenant);

            tenantSettingServiceMock
                .Setup(tenantSettingService => tenantSettingService.GetAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(tenantSetting);

            cultureServiceMock
                .Setup(cultureService => cultureService.GetAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(culture);

            var context = CreateValidationContextWithRequest(userName, tenantId.ToString());

            await target.ValidateAsync(context);

            Assert.IsNotNull(context.Result);
            Assert.IsFalse(context.Result.IsError);
            Assert.IsTrue(context.Result.Subject.Claims.Any(claim => claim.Type == InvoiceManagementClaims.TenantId && claim.Value == "Default"));
            Assert.IsTrue(context.Result.Subject.Claims.Any(claim => claim.Type == InvoiceManagementClaims.Culture && claim.Value == culture.Name));
        }

        private User CreateUser(string userName, int tenantId, string passwordHash)
        {
            return new User()
            {
                Id = tenantId,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                GroupId = tenantId,
                Username = userName
            };
        }

        private Tenant CreateTenant(string databaseName, int groupId, int tenantId)
        {
            return new Tenant
            {
                Id = tenantId,
                DatabaseName = databaseName,
                Status = TenantDatabaseStatus.Copying,
                GroupId = groupId
            };
        }

        private ResourceOwnerPasswordValidationContext CreateValidationContextWithRequest(string userName, string tenantId, string tenantIdKeyName = tenantIdKeyName)
        {
            var raw = new NameValueCollection();
            raw.Add(tenantIdKeyName, tenantId);

            return new ResourceOwnerPasswordValidationContext()
            {
                Password = "password",
                UserName = userName,
                Request = new ValidatedTokenRequest() { Raw = raw }
            };
        }

        private MockRepository mockRepository;
        private ResourceOwnerPasswordValidator target;
        private Mock<IPasswordHashService> passwordHashServiceMock;
        private Mock<IUserService> userServiceMock;
        private Mock<ITenantService> tenantServiceMock;
        private Mock<ITenantSettingService> tenantSettingServiceMock;
        private Mock<ICultureService> cultureServiceMock;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const string developerDatabaseName = "Invoices_Default";
        private const string invalidGrant = "invalid_grant";
        private const string tenantIdKeyName = "tenantId";
    }
}
