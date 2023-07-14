using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.UnitTests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.TenantCapture.Identity.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class PlanRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new PlanRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new PlanRepository(null));
        }

        [DataRow(-1)]
        [DataRow(0)]
        [TestMethod]
        public async Task GetByIdAsync_WhenPlanIdIsIncorrect_ShouldThrowArgumentExceptionAsync(int planId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => target.GetByIdAsync(planId, cancellationToken));
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnPlanAsync()
        {
            context.Plans.Add(new Plan() { Id = planId, CurrencyId = currencyId });
            context.Plans.Add(new Plan() { Id = 2, CurrencyId = currencyId });
            context.Plans.Add(new Plan() { Id = 33, CurrencyId = currencyId });
            context.Currencies.Add(new Currency() { Id = currencyId });
            context.SaveChanges();

            var actualPlan = await target.GetByIdAsync(planId, cancellationToken);

            Assert.IsNotNull(actualPlan);
            Assert.AreEqual(planId, actualPlan.Id);
        }

        [TestMethod]
        public async Task GetListAsync_ShouldReturnPlanAsync()
        {
            context.Plans.Add(new Plan() { Id = planId, CurrencyId = currencyId });
            context.Plans.Add(new Plan() { Id = planId2, CurrencyId = currencyId });
            context.Plans.Add(new Plan() { Id = planId3, CurrencyId = currencyId });
            context.Currencies.Add(new Currency() { Id = currencyId });
            context.SaveChanges();

            var actualPlans = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualPlans);
            Assert.AreEqual(3, actualPlans.Count);
            Assert.AreEqual(planId, actualPlans[0].Id);
            Assert.AreEqual(currencyId, actualPlans[0].CurrencyId);
            Assert.AreEqual(planId, actualPlans[0].Id);
            Assert.AreEqual(planId2, actualPlans[1].Id);
            Assert.AreEqual(planId3, actualPlans[2].Id);
        }

        [TestMethod]
        public async Task CreateAsync_WhenPlanNotNull_ShouldSavePlanAsync()
        {
            var expectedPlan = new Plan()
            {
                Name = planName,
                AllowedDocumentsCount = allowedDocumentsCount,
                CurrencyId = currencyId,
                Type = planType,
                Price = price
            };

            await target.CreateAsync(expectedPlan, cancellationToken);

            var actualPlan = context.Plans.FirstOrDefault();

            Assert.IsNotNull(actualPlan);
            Assert.AreNotEqual(0, actualPlan.Id);
            Assert.AreNotEqual(default, actualPlan.CreatedDate);
            Assert.AreNotEqual(default, actualPlan.ModifiedDate);

            Assert.AreEqual(planName, actualPlan.Name);
            Assert.AreEqual(planType, actualPlan.Type);
            Assert.AreEqual(price, actualPlan.Price);
            Assert.AreEqual(allowedDocumentsCount, actualPlan.AllowedDocumentsCount);
            Assert.AreEqual(currencyId, actualPlan.CurrencyId);
        }


        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private IPlanRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private const int planId = 22;
        private const int planId2 = 2;
        private const int planId3 = 33;
        private const int currencyId = 11;
        private const string planName = "somePlanName";
        private const int allowedDocumentsCount = 111;
        private const PlanType planType = PlanType.Monthly;
        private const decimal price = 17.5m;
    }
}
