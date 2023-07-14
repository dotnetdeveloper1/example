using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CultureServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            cultureRepositoryMock = mockRepository.Create<ICultureRepository>();
            target = new CultureService(cultureRepositoryMock.Object);
        }

        [TestMethod]
        public void Instance_WhenCultureRepositoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CultureService(null));
        }

        [TestMethod]
        public async Task GetListAsync_WhenCultureCollectionExists_ShouldReturnCultures()
        {
            var expectedCultures = new List<Culture>() { CreateCulture(1) };

            cultureRepositoryMock
                .Setup(cultureRepository => cultureRepository.GetListAsync(cancellationToken))
                .ReturnsAsync(expectedCultures);

            var actualCultures = await target.GetListAsync(cancellationToken);

            Assert.AreEqual(actualCultures.Count(), 1);
            AssertCulturesAreEqual(actualCultures[0], expectedCultures[0]);
        }

        [TestMethod]
        public async Task GetListAsync_WhenGroupRepositoryThrowError_ShouldThrowTimeoutException()
        {
            cultureRepositoryMock
                .Setup(groupRepository => groupRepository.GetListAsync(cancellationToken))
                .ThrowsAsync(new TimeoutException());

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.GetListAsync(cancellationToken));
        }
                
        private Culture CreateCulture(int id)
        {
            return new Culture()
            {
                Id = id,
                Name = $"Name{id}",
                EnglishName = $"EnglishName{id}",
                NativeName = $"NativeName{id}"
            };
        }

        private void AssertCulturesAreEqual(Culture actual, Culture expected)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.EnglishName, actual.EnglishName);
            Assert.AreEqual(expected.NativeName, actual.NativeName);
            Assert.AreEqual(expected.Id, actual.Id);
        }

        private GroupCreationParameters CreateGroupCreationParameters(string name, int? parentGroupId = null)
        {
            return new GroupCreationParameters()
            {
                Name = name,
                ParentGroupId = parentGroupId
            };
        }

        private MockRepository mockRepository;
        private Mock<ICultureRepository> cultureRepositoryMock;
        private CultureService target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
