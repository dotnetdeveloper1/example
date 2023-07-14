using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database
{
    internal class DatabaseContextFactory : IDatabaseContextFactory
    {
        public DatabaseContextFactory(IConnectionStringProvider connectionStringProvider) 
        {
            Guard.IsNotNull(connectionStringProvider, nameof(connectionStringProvider));

            this.connectionStringProvider = connectionStringProvider;
        }

        internal DatabaseContextFactory(DbContextOptions<DatabaseContext> options) 
        {
            Guard.IsNotNull(options, nameof(options));

            this.options = options;
        }

        public IDatabaseContext Create() => CreateContext(QueryTrackingBehavior.NoTracking);

        public IDatabaseContext CreateWithTracking() => CreateContext(QueryTrackingBehavior.TrackAll);

        private IDatabaseContext CreateContext(QueryTrackingBehavior trackingBehavior) 
        {
            var databaseOptions = options ?? CreateContextOptions();
            var context = new DatabaseContext(databaseOptions);
            context.ChangeTracker.QueryTrackingBehavior = trackingBehavior;

            return context;
        }

        private DbContextOptions<DatabaseContext> CreateContextOptions() 
        {
            var connectionString = connectionStringProvider.Get();
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return optionsBuilder.Options;
        }

        private readonly IConnectionStringProvider connectionStringProvider;

        // This field is used for test purposes only
        private readonly DbContextOptions<DatabaseContext> options;
    }
}
