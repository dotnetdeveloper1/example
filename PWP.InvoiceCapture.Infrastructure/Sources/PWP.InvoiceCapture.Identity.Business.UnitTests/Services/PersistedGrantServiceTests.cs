using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
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
    public class PersistedGrantServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            persistedGrantRepository = mockRepository.Create<IPersistedGrantRepository>();
            target = new PersistedGrantService(persistedGrantRepository.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenPersistedGrantRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PersistedGrantService(null));
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllAsync()
        {
            var expected = new List<PersistedGrant>() { new PersistedGrant() { Key = "key1" } };
            var actualFilters = new List<PersistedGrantFilter>();
            persistedGrantRepository
                .Setup(persistedGrant => persistedGrant.GetAllAsync(Capture.In(actualFilters)))
                .ReturnsAsync(expected);

            var actual = await target.GetAllAsync(new PersistedGrantFilter() { SessionId = "session", ClientId = "client", SubjectId = "subject", Type = "type" });

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(expected[0].Key, "key1");
            Assert.AreEqual(1, actualFilters.Count());
            Assert.AreEqual("session", actualFilters[0].SessionId);
            Assert.AreEqual("client", actualFilters[0].ClientId);
            Assert.AreEqual("subject", actualFilters[0].SubjectId);
            Assert.AreEqual("type", actualFilters[0].Type);
        }

        [TestMethod]
        public async Task GetAsync_ShouldReturnPersistedGrant()
        {
            var expected = new PersistedGrant() { Key = "key1" } ;
            var actualKeys = new List<string>();
            persistedGrantRepository
                .Setup(persistedGrant => persistedGrant.GetAsync(Capture.In(actualKeys)))
                .ReturnsAsync(expected);

            var actual = await target.GetAsync("key");

            Assert.IsNotNull(actual);
            Assert.AreEqual("key", actualKeys[0]);
            Assert.AreEqual(actual.Key, "key1");
        }

        [TestMethod]
        public async Task RemoveAllAsync_ShouldRemoveAllAsync()
        {
            var actualFilters = new List<PersistedGrantFilter>();
            persistedGrantRepository
                .Setup(persistedGrant => persistedGrant.RemoveAllAsync(Capture.In(actualFilters)))
                .Returns(Task.CompletedTask);

            await target.RemoveAllAsync(new PersistedGrantFilter() { SessionId = "session", ClientId = "client", SubjectId = "subject", Type = "type"  });

            Assert.AreEqual(1, actualFilters.Count());
            Assert.AreEqual("session", actualFilters[0].SessionId);
            Assert.AreEqual("client", actualFilters[0].ClientId);
            Assert.AreEqual("subject", actualFilters[0].SubjectId);
            Assert.AreEqual("type", actualFilters[0].Type);
        }

        [TestMethod]
        public async Task RemoveAsync_ShouldRemoveAsync()
        {
            var actual = new List<string>();
            persistedGrantRepository
                .Setup(persistedGrant => persistedGrant.RemoveAsync(Capture.In(actual)))
                .Returns(Task.CompletedTask);

            await target.RemoveAsync("key123");

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual("key123", actual[0]);
        }

        [TestMethod]
        public async Task StoreAsync_ShouldStoreAsync()
        {
            var actual = new List<PersistedGrant>();
            persistedGrantRepository
                .Setup(persistedGrant => persistedGrant.StoreAsync(Capture.In(actual)))
                .Returns(Task.CompletedTask);

            await target.StoreAsync(new PersistedGrant() { Key = "key1" });

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual("key1", actual[0].Key);
        }

        [TestMethod]
        public async Task RemoveAllExpiredPersistedGrantsAsync_ShouldRemoveAsync()
        {
            persistedGrantRepository
                .Setup(persistedGrant => persistedGrant.RemoveAllExpiredAsync(It.IsAny<DateTime>(), cancellationToken))
                .Returns(Task.CompletedTask);

            await target.RemoveAllExpiredPersistedGrantsAsync(cancellationToken);
        }

        private MockRepository mockRepository;
        private Mock<IPersistedGrantRepository> persistedGrantRepository;
        private PersistedGrantService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
