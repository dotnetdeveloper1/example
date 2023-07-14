using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.ServiceBus.Management;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.UnitTests.Management
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DefaultManagementClientFactoryTests
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task CreateAsync_WhenConnectionStringIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string connectionString) 
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                factory.CreateAsync(connectionString, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenConnectionStringIsValid_ShouldCreateManagementClient()
        {
            var managementClient = await factory.CreateAsync(connectionString, cancellationToken);

            Assert.IsNotNull(managementClient);
            Assert.IsInstanceOfType(managementClient, typeof(ManagementClientAdapter));
        }

        private readonly CancellationToken cancellationToken = new CancellationToken();
        private readonly DefaultManagementClientFactory factory = new DefaultManagementClientFactory();
        private readonly string connectionString = "Endpoint=sb://fake.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fakeAccessKey";
    }
}
