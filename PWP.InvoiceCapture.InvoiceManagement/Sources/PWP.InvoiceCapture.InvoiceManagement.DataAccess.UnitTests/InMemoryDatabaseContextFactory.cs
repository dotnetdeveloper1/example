using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.UnitTests
{
    [ExcludeFromCodeCoverage]
    internal class InMemoryDatabaseContextFactory : IDatabaseContextFactory
    {
        public InMemoryDatabaseContextFactory()
        {
            var databaseName = Guid.NewGuid().ToString();

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName)
                .UseInternalServiceProvider(serviceProvider);

            factory = new DatabaseContextFactory(optionsBuilder.Options);
        }

        public IDatabaseContext Create() => factory.Create();

        public IDatabaseContext CreateWithTracking() => factory.CreateWithTracking();

        private readonly IDatabaseContextFactory factory;
    }
}
