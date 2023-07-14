using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;

namespace PWP.InvoiceCapture.Identity.DataAccess.Database
{
    internal class MasterDatabaseContextFactory : IMasterDatabaseContextFactory
    {
        public MasterDatabaseContextFactory(IOptions<SqlManagementClientOptions> optionsAccessor)
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(optionsAccessor.Value, nameof(optionsAccessor.Value));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.MasterConnectionString, nameof(optionsAccessor.Value.MasterConnectionString));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.DefaultDatabaseName, nameof(optionsAccessor.Value.DefaultDatabaseName));
            Guard.IsNotZeroOrNegative(optionsAccessor.Value.CommandTimeoutInSeconds, nameof(optionsAccessor.Value.CommandTimeoutInSeconds));

            options = CreateContextOptions(optionsAccessor.Value);
        }

        internal MasterDatabaseContextFactory(DbContextOptions<MasterDatabaseContext> options)
        {
            Guard.IsNotNull(options, nameof(options));

            this.options = options;
        }

        public IMasterDatabaseContext Create() => CreateContext(QueryTrackingBehavior.NoTracking);

        private IMasterDatabaseContext CreateContext(QueryTrackingBehavior trackingBehavior)
        {
            var context = new MasterDatabaseContext(options);
            context.ChangeTracker.QueryTrackingBehavior = trackingBehavior;

            return context;
        }

        private DbContextOptions<MasterDatabaseContext> CreateContextOptions(SqlManagementClientOptions databaseOptions)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasterDatabaseContext>();
            optionsBuilder.UseSqlServer(databaseOptions.MasterConnectionString);

            return optionsBuilder.Options;
        }

        private readonly DbContextOptions<MasterDatabaseContext> options;
    }
}
