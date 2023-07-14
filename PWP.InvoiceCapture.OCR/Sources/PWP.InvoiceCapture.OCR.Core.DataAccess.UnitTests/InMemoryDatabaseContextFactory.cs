using PWP.InvoiceCapture.OCR.Core.DataAccess;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.OCR.DataAnalysis.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess.UnitTests
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

            var optionsBuilder = new DbContextOptionsBuilder<OCRDatabaseContext>()
                .UseInMemoryDatabase(databaseName)
                .UseInternalServiceProvider(serviceProvider);

            factory = new OCRDatabaseContextFactory(optionsBuilder.Options);
        }

        public IDatabaseContext Create() => factory.Create();

        public IDatabaseContext CreateWithTracking() => factory.CreateWithTracking();

        private readonly IDatabaseContextFactory factory;
    }
}
