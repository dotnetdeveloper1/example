using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class WebhookRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            context = (DatabaseContext)contextFactory.Create();
            target = new WebhookRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new WebhookRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenWebhookIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        [DataRow(TriggerType.StatusChanged)]
        public async Task CreateAsync_WhenRequestIsNotNull_ShouldSaveWebhookRequest(TriggerType triggerType)
        {
            var request = CreateRequest(triggerType, 0);

            await target.CreateAsync(request, cancellationToken);

            var actualRequest = context.Webhooks.FirstOrDefault();

            Assert.IsNotNull(actualRequest);
            Assert.AreNotEqual(0, actualRequest.Id);
            Assert.AreNotEqual(default, actualRequest.CreatedDate);
            Assert.AreNotEqual(default, actualRequest.ModifiedDate);

            AssertWebhooksAreEqual(request, actualRequest);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenRequestIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.UpdateAsync(1, null, cancellationToken));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task UpdateAsync_WhenRequestIdIsLessOrEqualsZero_ShouldThrowArgumentException(int requestId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.UpdateAsync(requestId, new Webhook(), cancellationToken));
        }

        [TestMethod]
        [DataRow(TriggerType.StatusChanged, 1)]
        [DataRow(TriggerType.StatusChanged, 2)]
        public async Task UpdateAsync_WhenInvoiceFieldIsNotNull_ShouldUpdateFieldAndModifiedDate(TriggerType triggerType, int requestId)
        {
            var existingRequest = CreateRequest(triggerType, requestId);

            context.Webhooks.Add(existingRequest);
            context.SaveChanges();

            var updatedRequest = CreateRequest(triggerType, requestId);
            updatedRequest.Url = "UpdatedUrl";

            await target.UpdateAsync(updatedRequest.Id, updatedRequest, cancellationToken);

            var actualRequest = context.Webhooks.FirstOrDefault();

            Assert.IsNotNull(actualRequest);
            Assert.AreNotEqual(default, actualRequest.ModifiedDate);

            AssertWebhooksAreEqual(updatedRequest, actualRequest);
        }

        [TestMethod]
        [DataRow(TriggerType.StatusChanged, 10)]
        [DataRow(TriggerType.StatusChanged, 5)]
        public async Task GetListAsync_WhenRequestsCollectionIsNotEmpty_ShouldReturnList(TriggerType triggerType, int count)
        {
            var invoiceRequests = Enumerable
                .Range(1, count)
                .Select(index => CreateRequest(triggerType, index))
                .ToList();

            context.Webhooks.AddRange(invoiceRequests);
            context.SaveChanges();

            var actualWebhooks = await target.GetListAsync(cancellationToken);

            Assert.IsNotNull(actualWebhooks);
            Assert.AreEqual(count, actualWebhooks.Count);
        }

        [TestMethod]
        [DataRow(TriggerType.StatusChanged, 10)]
        [DataRow(TriggerType.StatusChanged, 5)]
        public async Task GetAsync_WhenRequestsCollectionIsNotEmpty_ShouldReturnWebhook(TriggerType triggerType, int count)
        {
            var invoiceRequests = Enumerable
                .Range(1, count)
                .Select(index => CreateRequest(triggerType, index))
                .ToList();

            context.Webhooks.AddRange(invoiceRequests);
            context.SaveChanges();

            int id = count - 1;

            var actualWebhook = await target.GetAsync(id, cancellationToken);

            Assert.IsNotNull(actualWebhook);
            AssertWebhooksAreEqual(context.Webhooks.First(hook => hook.Id == id), actualWebhook);
        }

        [TestMethod]
        [DataRow(TriggerType.StatusChanged, 10)]
        [DataRow(TriggerType.StatusChanged, 5)]
        public async Task AnyAsync_WhenContainsWebhookWithSameTriggerTypeAndUrl_ShouldReturnTrue(TriggerType triggerType, int count)
        {
            var invoiceRequests = Enumerable
                .Range(1, count)
                .Select(index => CreateRequest(triggerType, index, $"url{index}"))
                .ToList();

            context.Webhooks.AddRange(invoiceRequests);
            context.SaveChanges();

            int id = count - 1;

            var hasSameWebhook = await target.AnyAsync(triggerType, $"url{id}", cancellationToken);

            Assert.IsTrue(hasSameWebhook);
        }

        [TestMethod]
        [DataRow(TriggerType.StatusChanged, 10)]
        [DataRow(TriggerType.StatusChanged, 5)]
        public async Task AnyAsync_WhenContainsSameWebhookWithOtherId_ShouldReturnTrue(TriggerType triggerType, int count)
        {
            var invoiceRequests = Enumerable
                .Range(1, count)
                .Select(index => CreateRequest(triggerType, index, $"url{index}"))
                .ToList();

            context.Webhooks.AddRange(invoiceRequests);
            context.SaveChanges();

            var webhookIdToUpdate = count - 1;
            var sameUrl = $"url{webhookIdToUpdate - 1}";

            var hasSameWebhook = await target.AnyAsync(triggerType, sameUrl, webhookIdToUpdate, cancellationToken);

            Assert.IsTrue(hasSameWebhook);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task DeleteAsync_WhenRequestIdIsLessOrEqualsZero_ShouldThrowArgumentException(int requestId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                target.DeleteAsync(requestId, cancellationToken));
        }

        [TestMethod]
        [DataRow(TriggerType.StatusChanged, 10)]
        [DataRow(TriggerType.StatusChanged, 5)]
        public async Task DeleteAsync_WhenIdIsCorrect_ShouldDeleteInvoiceField(TriggerType triggerType, int count)
        {
            var invoiceRequests = Enumerable
                .Range(1, count)
                .Select(index => CreateRequest(triggerType, index))
                .ToList();

            context.Webhooks.AddRange(invoiceRequests);
            context.SaveChanges();

            await target.DeleteAsync(invoiceRequests[0].Id, cancellationToken);

            var deletedInvoice = context.Webhooks.FirstOrDefault(invoiceField => invoiceField.Id == invoiceRequests[0].Id);
            Assert.IsNull(deletedInvoice);
            Assert.AreEqual(invoiceRequests.Count() - 1, context.Webhooks.Count());
        }

        private void AssertWebhooksAreEqual(Webhook expected, Webhook actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.TriggerType, actual.TriggerType);
            Assert.AreEqual(expected.Url, actual.Url);

            // Ensure all properties are tested
            Assert.AreEqual(5, actual.GetType().GetProperties().Length);
        }

        private Webhook CreateRequest(TriggerType triggerType, int id)
        {
            return new Webhook
            {
                Id = id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                TriggerType = triggerType,
                Url = "url"
            };
        }

        private Webhook CreateRequest(TriggerType triggerType, int id, string url)
        {
            return new Webhook
            {
                Id = id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                TriggerType = triggerType,
                Url = url
            };
        }

        private DatabaseContext context;
        private IDatabaseContextFactory contextFactory;
        private IWebhookRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
