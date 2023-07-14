using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.UnitTests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.TenantCapture.Identity.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class GroupPlanRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new GroupPlanRepository(contextFactory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod]
        public void Instance_WhenDbContextIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new GroupPlanRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenGroupPlanIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task CreateAsync_WhenGroupPlanNotNull_ShouldSaveGroupPlanAsync(int groupPlanId)
        {
            var expectedGroupPlan = CreateGroupPlan(groupPlanId);
           
                       
            await target.CreateAsync(expectedGroupPlan, cancellationToken);

            var actualGroupPlan = context.GroupPlans.FirstOrDefault();

            Assert.IsNotNull(actualGroupPlan);
            Assert.AreNotEqual(0, actualGroupPlan.Id);
            Assert.AreNotEqual(default, actualGroupPlan.CreatedDate);
            Assert.AreNotEqual(default, actualGroupPlan.ModifiedDate);

            AssertGroupPlansAreEqual(expectedGroupPlan, actualGroupPlan, false);
        }

        [TestMethod]
        public async Task GetListAsync_WhenPlansCollectionIsEmpty_ShouldReturnEmptyListAsync()
        {
            var actualPlans = await target.GetListAsync(1, cancellationToken);

            Assert.IsNotNull(actualPlans);
            Assert.AreEqual(0, actualPlans.Count);
        }

        [TestMethod]
        public async Task GetListAsync_WhenPlansCollectionIsNotEmpty_ShouldReturnAllAsync()
        {
            var groupPlan = CreateGroupPlan(1);
            context.GroupPlans.Add(groupPlan);
            context.Groups.Add(new Group() { Id = groupId});
            context.Currencies.Add(new Currency() { Id = currencyId });
            context.Plans.Add(new Plan() { Id = planId, CurrencyId = currencyId });
            context.SaveChanges();

            var actualPlans = await target.GetListAsync(groupId, cancellationToken);

            Assert.IsNotNull(actualPlans);
            Assert.AreEqual(1, actualPlans.Count);
            AssertGroupPlansAreEqual(groupPlan, actualPlans[0]);
        }

        [DataRow(-1)]
        [DataRow(0)]
        [TestMethod]
        public async Task GetListAsync_WhenPlanIdIsIncorrect_ShouldThrowArgumentExceptionAsync(int planId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetListAsync(planId, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsyncc_GroupPlanIdIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.CreateAsync(null, cancellationToken));
        }

        [DataRow(-1)]
        [DataRow(0)]
        [TestMethod]
        public async Task GetActiveAsync_GroupIdIsIncorrect_ShouldThrowArgumentxceptionAsync(int groupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetActiveAsync(groupId, DateTime.Now, cancellationToken));
        }

        [TestMethod]
        public async Task UpdateAsync_GroupPlanIsNull_ShouldThrowArgumentNullExceptionAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.UpdateAsync(null, cancellationToken));
        }


        [DataRow(-1)]
        [DataRow(0)]
        [TestMethod]
        public async Task IsIntersectAsync_GroupIdIsIncorrect_ShouldThrowArgumentNullExceptionAsync(int groupId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.IsIntersectAsync(groupId, DateTime.Now, DateTime.Now, cancellationToken));
        }

        [DataRow("2020-01-01", "2020-01-04")]
        [DataRow("2020-01-04", "2020-01-05")]
        [DataRow("2020-01-05", "2020-04-01")]
        [DataRow("2020-01-01", "2020-04-01")]
        [TestMethod]
        public async Task IsIntersectAsync_WhenIntersect_ShouldReturnTrueAsync(string startDate, string endDate)
        {
            context.Plans.Add(new Plan() { Id = planId, CurrencyId = currencyId });
            context.GroupPlans.Add(new GroupPlan()
            {
                StartDate = DateTime.Parse("2020-01-03", CultureInfo.InvariantCulture),
                EndDate = DateTime.Parse("2020-03-01", CultureInfo.InvariantCulture),
                GroupId = groupId
            });
            context.SaveChanges();

            var isIntersects = await target.IsIntersectAsync(groupId, DateTime.Parse(startDate, CultureInfo.InvariantCulture),
                DateTime.Parse(endDate, CultureInfo.InvariantCulture), cancellationToken);

            Assert.AreEqual(true, isIntersects);
        }

        [DataRow("2020-01-01", "2020-01-02")]
        [DataRow("2020-04-01", "2020-05-01")]
        [TestMethod]
        public async Task IsIntersectAsync_WhenNotIntersect_ShouldReturnFalseAsync(string startDate, string endDate)
        {
            context.Plans.Add(new Plan() { Id = planId, CurrencyId = currencyId });
            context.GroupPlans.Add(new GroupPlan()
            {
                StartDate = DateTime.Parse("2020-01-03", CultureInfo.InvariantCulture),
                EndDate = DateTime.Parse("2020-03-01", CultureInfo.InvariantCulture),
                GroupId = groupId
            });
            context.SaveChanges();

            var isIntersects = await target.IsIntersectAsync(groupId, DateTime.Parse(startDate, CultureInfo.InvariantCulture),
                DateTime.Parse(endDate,CultureInfo.InvariantCulture), cancellationToken);

            Assert.AreEqual(false, isIntersects);
        }

        [TestMethod]
        public async Task GetActiveAsync_WhenPlanActive_ShouldReturnOnlyActiveAsync()
        {
            context.Plans.Add(new Plan() { Id = planId, CurrencyId = currencyId, AllowedDocumentsCount = 10});
            context.Currencies.Add(new Currency() { Id = currencyId });
            context.Groups.Add(new Group() { Id = groupId });
            context.GroupPlans.Add(new GroupPlan()
            {
                Id = 11,
                StartDate = DateTime.Parse("2020-01-01", CultureInfo.InvariantCulture),
                EndDate = DateTime.Parse("2020-03-01", CultureInfo.InvariantCulture),
                GroupId = groupId,
                PlanId = planId,
                UploadedDocumentsCount = 2
            });
           
            context.GroupPlans.Add(new GroupPlan()
            {
                Id = 14,
                StartDate = DateTime.Parse("2020-04-03", CultureInfo.InvariantCulture),
                EndDate = DateTime.Parse("2020-05-01", CultureInfo.InvariantCulture),
                GroupId = groupId,
                PlanId = planId
            });
            context.SaveChanges();

            var result = await target.GetActiveAsync(groupId, DateTime.Parse("2020-02-03", CultureInfo.InvariantCulture),
                cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(11, result.Id);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldIncreaseAsync()
        {
            var groupPlan = new GroupPlan()
            {
                Id = groupPlanId,
                UploadedDocumentsCount = uploadedCount
            };
            var expectedCount = uploadedCount + 1;
            context.GroupPlans.Add(groupPlan);
            context.SaveChanges();
            groupPlan.UploadedDocumentsCount = expectedCount;
            await target.UpdateAsync(groupPlan, cancellationToken);

            Assert.AreEqual(expectedCount, context.GroupPlans.FirstOrDefault(groupPlan => groupPlan.Id == groupPlanId).UploadedDocumentsCount);
        }

        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-5)]
        [TestMethod]
        public async Task DeleteAsync_GroupPlanIdIsIncorrect_ShouldThrowArgumentExceptionAsync(int groupPlanId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.DeleteAsync(groupPlanId, cancellationToken));
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteAsync()
        {
            var groupPlans = Enumerable
                .Range(1, 3)
                .Select(index => CreateGroupPlan(index))
                .ToList();
            context.GroupPlans.AddRange(groupPlans);
            context.SaveChanges();

            await target.DeleteAsync(groupPlans.FirstOrDefault(groupPlans => groupPlans.Id == 2).Id, cancellationToken);

            Assert.AreEqual(2, context.GroupPlans.Count());
            Assert.AreEqual(1, context.GroupPlans.ToArray()[0].Id);
            Assert.AreEqual(3, context.GroupPlans.ToArray()[1].Id);
        }

        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-5)]
        [TestMethod]
        public async Task GetByIdAsync_GroupPlanIdIsIncorrect_ShouldThrowArgumentExceptionAsync(int groupPlanId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetByIdAsync(groupPlanId, cancellationToken));
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnGroupPlanAsync()
        {
            var groupPlans = Enumerable
                .Range(1, 3)
                .Select(index => CreateGroupPlan(index))
                .ToList();

            context.Groups.Add(new Group() { Id = groupId });
            context.Currencies.Add(new Currency() { Id = currencyId });
            context.Plans.Add(new Plan() { Id = planId, CurrencyId = currencyId });
            context.GroupPlans.AddRange(groupPlans);
            context.SaveChanges();

            var result = await target.GetByIdAsync(2, cancellationToken);

            Assert.IsNotNull(result);
            AssertGroupPlansAreEqual(context.GroupPlans.ToArray()[1], result);
        }

        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-5)]
        [TestMethod]
        public async Task CancelRenewalAsync_GroupPlanIdIsIncorrect_ShouldThrowArgumentExceptionAsync(int groupPlanId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.CancelRenewalAsync(groupPlanId, cancellationToken));
        }

        [TestMethod]
        public async Task CancelRenewalAsync_ShouldReturnGroupPlanAsync()
        {
            var groupPlans = Enumerable
                .Range(1, 3)
                .Select(index => CreateGroupPlan(index))
                .ToList();

            context.Groups.Add(new Group() { Id = groupId });
            context.Currencies.Add(new Currency() { Id = currencyId });
            context.Plans.Add(new Plan() { Id = planId, CurrencyId = currencyId });
            context.GroupPlans.AddRange(groupPlans);
            context.SaveChanges();

            await target.CancelRenewalAsync(2, cancellationToken);
            var expectedUpdated = groupPlans.ToArray()[1];
            expectedUpdated.IsRenewalCancelled = true;
            
            AssertGroupPlansAreEqual(context.GroupPlans.ToArray()[0], groupPlans.ToArray()[0]);
            AssertGroupPlansAreEqual(context.GroupPlans.ToArray()[2], groupPlans.ToArray()[2]);
            AssertGroupPlansAreEqual(expectedUpdated, groupPlans.ToArray()[1]);
        }

        private GroupPlan CreateGroupPlan(int id)
        {
            return new GroupPlan
            {
                Id = id,
                PlanId = planId,
                GroupId = groupId,
                UploadedDocumentsCount = 0,
                StartDate = DateTime.Parse("2020-01-03", CultureInfo.InvariantCulture),
                EndDate = DateTime.Parse("2020-02-03", CultureInfo.InvariantCulture),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
        }

        private void AssertGroupPlansAreEqual(GroupPlan expected, GroupPlan actual , bool shouldIncludesBeChecked = true)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.PlanId, actual.PlanId);
            Assert.AreEqual(expected.GroupId, actual.GroupId);
            Assert.AreEqual(expected.UploadedDocumentsCount, actual.UploadedDocumentsCount);
            Assert.AreEqual(expected.StartDate, actual.StartDate);
            Assert.AreEqual(expected.EndDate, actual.EndDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);

            if (shouldIncludesBeChecked)
            {
                AssertGroupPlanIncludes(actual);
            }
        }

        private void AssertGroupPlanIncludes(GroupPlan actual)
        {
            Assert.IsNotNull(actual.Plan);
            Assert.IsNotNull(actual.Plan.Currency);
            Assert.AreEqual(planId, actual.Plan.Id);
            Assert.AreEqual(currencyId, actual.Plan.Currency.Id);
        }

        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private IGroupPlanRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int planId = 22;
        private const int currencyId = 44;
        private const int groupId = 33;
        private const int groupPlanId = 333;
        private const int uploadedCount = 40;
    }
}
