using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Identity.DataAccess.UnitTests
{
    [ExcludeFromCodeCoverage]
    internal class InMemoryTenantsDatabaseContextFactory : ITenantsDatabaseContextFactory
    {
        public InMemoryTenantsDatabaseContextFactory()
        {
            var databaseName = Guid.NewGuid().ToString();

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<TenantsDatabaseContext>()
                .UseInMemoryDatabase(databaseName)
                .UseInternalServiceProvider(serviceProvider);

            factory = new TenantsDatabaseContextFactory(optionsBuilder.Options);
        }

        public ITenantsDatabaseContext Create() => factory.Create();

        public ITenantsDatabaseContext CreateWithTracking() => factory.CreateWithTracking();

        private readonly ITenantsDatabaseContextFactory factory;
    }
}
