using IdentityServer4.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using PWP.InvoiceCapture.Identity.DataAccess.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.UnitTests;
using IdentityServer4.Stores;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace PWP.TenantCapture.Identity.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class PersistedGrantRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryTenantsDatabaseContextFactory();
            context = (TenantsDatabaseContext)contextFactory.Create();
            target = new PersistedGrantRepository(contextFactory);
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
            Assert.ThrowsException<ArgumentNullException>(() => new PersistedGrantRepository(null));
        }

        [TestMethod]
        public async Task GetAsync_ShouldReturnPersistedGrantAsync()
        {
            context.PersistedGrants.Add(CreatePersistedGrant());
            context.PersistedGrants.Add(new PersistedGrant() { Key = "2"});
            context.SaveChanges();

            var actualPersistedGrant = await target.GetAsync(key);
            CheckPersistedGrants(actualPersistedGrant);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnPersistedGrantsAsync()
        {
            context.PersistedGrants.Add(CreatePersistedGrant());
            context.PersistedGrants.Add(new PersistedGrant() { Key = "2" });
            context.SaveChanges();

            var actualPersistedGrants = await target.GetAllAsync(new PersistedGrantFilter());

            Assert.IsNotNull(actualPersistedGrants);
            Assert.AreEqual(2, actualPersistedGrants.Count());
            CheckPersistedGrants(actualPersistedGrants.First());
        }

        [TestMethod]
        public async Task RemoveAllAsync_ShouldRemovePersistedGrantsByTypeAsync()
        {
            var firstItem = CreatePersistedGrant();
            var secondItem = CreatePersistedGrant("key2", "type2");
            context.PersistedGrants.Add(firstItem);
            context.PersistedGrants.Add(secondItem);
            context.SaveChanges();

            await target.RemoveAllAsync(new PersistedGrantFilter() { Type = type});

            var actual = context.PersistedGrants.ToList();

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual("key2", actual[0].Key);
        }

        [TestMethod]
        public async Task RemoveAllAsync_ShouldRemovePersistedGrantsByClientIdTypeAsync()
        {
            var firstItem = CreatePersistedGrant();
            var secondItem = CreatePersistedGrant("key2", type, "clientId2");
            context.PersistedGrants.Add(firstItem);
            context.PersistedGrants.Add(secondItem);
            context.SaveChanges();

            await target.RemoveAllAsync(new PersistedGrantFilter() { ClientId = "clientId2" });

            var actual = context.PersistedGrants.ToList();

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(key, actual[0].Key);
        }

        [TestMethod]
        public async Task RemoveAllAsync_ShouldRemovePersistedGrantsBySubjectIdTypeAsync()
        {
            var firstItem = CreatePersistedGrant();
            var secondItem = CreatePersistedGrant("key2", type, clientId, "subjectId2");
            context.PersistedGrants.Add(firstItem);
            context.PersistedGrants.Add(secondItem);
            context.SaveChanges();

            await target.RemoveAllAsync(new PersistedGrantFilter() { SubjectId = "subjectId2" });

            var actual = context.PersistedGrants.ToList();

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(key, actual[0].Key);
        }

        [TestMethod]
        public async Task RemoveAllAsync_ShouldRemovePersistedGrantsBySessionIdTypeAsync()
        {
            var firstItem = CreatePersistedGrant();
            var secondItem = CreatePersistedGrant("key2", type, clientId, "subjectId2", "sessionId");
            context.PersistedGrants.Add(firstItem);
            context.PersistedGrants.Add(secondItem);
            context.SaveChanges();

            await target.RemoveAllAsync(new PersistedGrantFilter() { SessionId = "sessionId" });

            var actual = context.PersistedGrants.ToList();

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(key, actual[0].Key);
        }

        [TestMethod]
        public async Task RemoveAllAsync_ShouldRemovePersistedGrantsBySessionIdAndSubjectIdTypeAsync()
        {
            var firstItem = CreatePersistedGrant();
            var secondItem = CreatePersistedGrant("key2", type, clientId, "subjectId2", "sessionId123");
            var thirdItem = CreatePersistedGrant("key3", "type3", "client3", "subjectId2", "sessionId123");
            var fourthItem = CreatePersistedGrant("key4", "type4", "client4", "subjectId4", "sessionId123");
            context.PersistedGrants.Add(firstItem);
            context.PersistedGrants.Add(secondItem);
            context.PersistedGrants.Add(thirdItem);
            context.PersistedGrants.Add(fourthItem);
            context.SaveChanges();

            await target.RemoveAllAsync(new PersistedGrantFilter() { SessionId = "sessionId123",  SubjectId = "subjectId2" });

            var actual = context.PersistedGrants.ToList();

            Assert.AreEqual(2, actual.Count());
            Assert.AreEqual(key, actual[0].Key);
            Assert.AreEqual("key4", actual[1].Key);
        }

        [TestMethod]
        public async Task RemoveAsync_ShouldRemovePersistedGrantsAsync()
        {
            var firstItem = CreatePersistedGrant();
            var secondItem = CreatePersistedGrant("key2", type, clientId, "subjectId2", "sessionId123");
            var thirdItem = CreatePersistedGrant("key3", "type3", "client3", "subjectId2", "sessionId123");
            var fourthItem = CreatePersistedGrant("key4", "type4", "client4", "subjectId4", "sessionId123");
            context.PersistedGrants.Add(firstItem);
            context.PersistedGrants.Add(secondItem);
            context.PersistedGrants.Add(thirdItem);
            context.PersistedGrants.Add(fourthItem);
            context.SaveChanges();

            await target.RemoveAsync("key4");

            var actual = context.PersistedGrants.ToList();

            Assert.AreEqual(3, actual.Count());
            Assert.AreEqual(key, actual[0].Key);
            Assert.AreEqual("key2", actual[1].Key);
            Assert.AreEqual("key3", actual[2].Key);
        }

        [TestMethod]
        public async Task StoreAsync_ShouldAddAsync()
        {
            var firstItem = CreatePersistedGrant();
            context.PersistedGrants.Add(firstItem);
            context.SaveChanges();

            await target.StoreAsync(CreatePersistedGrant("key2"));

            var actual = context.PersistedGrants.ToList();

            Assert.AreEqual(2, actual.Count());
            Assert.AreEqual(key, actual[0].Key);
            Assert.AreEqual("key2", actual[1].Key);
        }

        [TestMethod]
        public async Task RemoveAllExpiredAsync_ShouldRemoveAsync()
        {
            var firstItem = CreatePersistedGrant();
            var secondItem = CreatePersistedGrant("key2");
            var thirdItem = CreatePersistedGrant("key3");
            thirdItem.Expiration = new DateTime(2020, 10, 10);
            context.PersistedGrants.Add(firstItem);
            context.PersistedGrants.Add(secondItem);
            context.PersistedGrants.Add(thirdItem);
            context.SaveChanges();

            await target.RemoveAllExpiredAsync(new DateTime(2020, 1, 5), CancellationToken.None);

            var actual = context.PersistedGrants.ToList();

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual("key3", actual[0].Key);
        }

        private PersistedGrant CreatePersistedGrant(string key = key, string type = type, string clientId = clientId,
            string subjectId = subjectId, string sessionId = sessionId)
        {
            return new  PersistedGrant()
            {
                Key = key,
                Type = type,
                SubjectId = subjectId,
                SessionId = sessionId,
                ClientId = clientId,
                Description = description,
                Data = data,
                Expiration = expiration,
                ConsumedTime = consumed,
                CreationTime = creation
            };
        }

        private void CheckPersistedGrants(PersistedGrant persistedGrant)
        {
            Assert.IsNotNull(persistedGrant);

            Assert.AreEqual(key, persistedGrant.Key);
            Assert.AreEqual(type, persistedGrant.Type);
            Assert.AreEqual(clientId, persistedGrant.ClientId);
            Assert.AreEqual(sessionId, persistedGrant.SessionId);
            Assert.AreEqual(subjectId, persistedGrant.SubjectId);
            Assert.AreEqual(description, persistedGrant.Description);
            Assert.AreEqual(data, persistedGrant.Data);
            Assert.AreEqual(creation, persistedGrant.CreationTime);
            Assert.AreEqual(expiration, persistedGrant.Expiration);
            Assert.AreEqual(consumed, persistedGrant.ConsumedTime);
        }


        private TenantsDatabaseContext context;
        private ITenantsDatabaseContextFactory contextFactory;
        private IPersistedGrantRepository target;
        private const string key = "someKey";
        private const string type = "someType";
        private const string subjectId = "someSubjectId";
        private const string sessionId = "someSessionId";
        private const string clientId = "someClientId";
        private const string description = "someDescription";
        private const string data = "someData";
        private readonly DateTime creation = new DateTime(2020, 1, 1);
        private readonly DateTime expiration = new DateTime(2020, 1, 2);
        private readonly DateTime consumed = new DateTime(2020, 1, 3);
    }
}
