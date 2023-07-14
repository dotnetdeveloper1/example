using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using PWP.InvoiceCapture.Identity.DataAccess.Database;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Identity.DataAccess.UnitTests
{
    [ExcludeFromCodeCoverage]
    internal class InMemoryMasterDatabaseContextFactory : IMasterDatabaseContextFactory
    {
        public InMemoryMasterDatabaseContextFactory()
        {
            var databaseName = Guid.NewGuid().ToString();

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<MasterDatabaseContext>()
                .UseInMemoryDatabase(databaseName)
                .UseInternalServiceProvider(serviceProvider);

            factory = new MasterDatabaseContextFactory(optionsBuilder.Options);
        }

        public IMasterDatabaseContext Create() => factory.Create();

        private readonly IMasterDatabaseContextFactory factory;
    }
}
