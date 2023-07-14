using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.ServiceBus.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DefaultMessageIdentityProviderTests
    {
        [TestMethod]
        public async Task GetNextAsync_ShouldReturnIdentifier() 
        {
            var id = await messageIdentityProvider.GetNextAsync(cancellationToken);
            
            Assert.IsFalse(string.IsNullOrWhiteSpace(id));
            Assert.IsTrue(Guid.TryParse(id, out _));
        }

        [TestMethod]
        public async Task GetNextAsync_WhenRequestedMultipleTimes_ShouldReturnUniqueIdentifier()
        {
            var idTasks = Enumerable
                .Range(1, 100)
                .Select((number) => messageIdentityProvider.GetNextAsync(cancellationToken));

            var ids = await Task.WhenAll(idTasks);

            Assert.IsNotNull(ids);
            CollectionAssert.AllItemsAreNotNull(ids);
            CollectionAssert.AllItemsAreUnique(ids);
        }

        private readonly DefaultMessageIdentityProvider messageIdentityProvider = new DefaultMessageIdentityProvider();
        private readonly CancellationToken cancellationToken = new CancellationToken();
    }
}
