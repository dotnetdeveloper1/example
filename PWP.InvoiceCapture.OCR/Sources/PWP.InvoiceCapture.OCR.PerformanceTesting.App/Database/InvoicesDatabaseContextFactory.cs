using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Database
{
    internal class InvoicesDatabaseContextFactory : IInvoicesDatabaseContextFactory
    {
        public InvoicesDatabaseContextFactory(IInvoicesDatabaseConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        public InvoicesDatabaseContext Create() 
        {
            var options = CreateContextOptions();
            var context = new InvoicesDatabaseContext(options);
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return context;
        }

        private DbContextOptions<InvoicesDatabaseContext> CreateContextOptions()
        {
            var connectionString = connectionStringProvider.Get();
            var optionsBuilder = new DbContextOptionsBuilder<InvoicesDatabaseContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return optionsBuilder.Options;
        }

        private readonly IInvoicesDatabaseConnectionStringProvider connectionStringProvider;
    }
}
