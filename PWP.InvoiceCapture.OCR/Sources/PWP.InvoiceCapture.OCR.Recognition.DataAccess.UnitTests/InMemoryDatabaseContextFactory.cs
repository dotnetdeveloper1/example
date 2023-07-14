using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Contracts;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Database;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.UnitTests
{
    [ExcludeFromCodeCoverage]
    internal class InMemoryRecognitionDatabaseContextFactory : IRecognitionDatabaseContextFactory
    {
        public InMemoryRecognitionDatabaseContextFactory()
        {
            var databaseName = Guid.NewGuid().ToString();

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<RecognitionDatabaseContext>()
                .UseInMemoryDatabase(databaseName)
                .UseInternalServiceProvider(serviceProvider);

            factory = new RecognitionDatabaseContextFactory(optionsBuilder.Options);
        }

        public IRecognitionDatabaseContext Create() => factory.Create();

        public IRecognitionDatabaseContext CreateWithTracking() => factory.CreateWithTracking();

        private readonly IRecognitionDatabaseContextFactory factory;
    }
}
