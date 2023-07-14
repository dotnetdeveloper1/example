using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class GroupRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            cancellationToken = CancellationToken.None;
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new GroupRepository(contextFactory);
        }

        [TestMethod]
        public void Instance_WhenDbContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new GroupRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_ShouldSaveGroupAsync()
        {
            var group = CreateGroup(1, 2);

            await target.CreateAsync(group, cancellationToken);

            var result = context.Groups.ToList();

            Assert.AreEqual(result.Count, 1);

            AssertGroupsAreEqual(group, result[0], false);
        }

        [TestMethod]
        public async Task CreateAsync_WhenUserIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            Group group = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(group, cancellationToken));
        }

        [TestMethod]
        [DataRow(100)]
        public async Task GetListAsync_WhenGroupsCollectionIsNotEmpty_ShouldReturnAllAsync(int count)
        {
            var groups = Enumerable
                .Range(0, count)
                .Select(index => CreateGroup(index + 1, index + 2))
                .ToList();

            context.Groups.AddRange(groups);
            context.SaveChanges();

            var actualGroups = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualGroups);
            Assert.AreEqual(count, actualGroups.Count);
            groups.ForEach(group => AssertGroupsAreEqual(group, actualGroups.FirstOrDefault(actualGroup => actualGroup.Id == group.Id), true));
        }

        [TestMethod]
        [DataRow(5)]
        [DataRow(10)]
        public async Task ExistsAsync_WhenGroupWithIdNotFound_ShouldReturnFalseAsync(int groupId)
        {
            context.Groups.Add(CreateGroup(groupId - 1, groupId));
            context.SaveChanges();

            Assert.AreEqual(context.Groups.Count(), 1);

            var groupExists = await target.ExistsAsync(groupId, cancellationToken);

            Assert.IsFalse(groupExists);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        public async Task ExistsAsync_WhenGroupWithIdExists_ShouldReturnTrueAsync(int groupId)
        {
            var expectedGroup = CreateGroup(groupId, groupId + 1);

            context.Groups.Add(expectedGroup);

            context.SaveChanges();

            Assert.AreEqual(1, context.Groups.Count());

            var isGroupExists = await target.ExistsAsync(expectedGroup.Id, cancellationToken);

            Assert.IsTrue(isGroupExists);
        }

        private Group CreateGroup(int groupId, int tenantId)
        {
            return new Group
            {
                Id = groupId,
                ParentGroupId = null,
                Name = $"Name_{groupId}",
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Tenants = new List<Tenant>
                {
                    new Tenant { Id = tenantId, GroupId = groupId }
                }
            };
        }

        private void AssertGroupsAreEqual(Group expected, Group actual, bool checkTenants)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ParentGroupId, actual.ParentGroupId);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            if (checkTenants)
            {
                Assert.AreEqual(expected.Tenants[0].Id, actual.Tenants[0].Id);
            }

            Assert.AreEqual(6, actual.GetType().GetProperties().Length);
        }

        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private IGroupRepository target;
        private CancellationToken cancellationToken;
    }
}
