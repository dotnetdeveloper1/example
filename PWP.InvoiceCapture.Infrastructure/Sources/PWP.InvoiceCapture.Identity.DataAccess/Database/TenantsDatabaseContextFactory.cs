using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Options;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;

namespace PWP.InvoiceCapture.Identity.DataAccess.Database
{
    internal class TenantsDatabaseContextFactory : ITenantsDatabaseContextFactory
    {
        public TenantsDatabaseContextFactory(IOptions<DatabaseOptions> optionsAccessor)
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(optionsAccessor.Value, nameof(optionsAccessor.Value));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.ConnectionString, nameof(optionsAccessor.Value.ConnectionString));

            options = CreateContextOptions(optionsAccessor.Value);
        }

        internal TenantsDatabaseContextFactory(DbContextOptions<TenantsDatabaseContext> options)
        {
            Guard.IsNotNull(options, nameof(options));

            this.options = options;
        }

        public ITenantsDatabaseContext Create() => CreateContext(QueryTrackingBehavior.NoTracking);

        public ITenantsDatabaseContext CreateWithTracking() => CreateContext(QueryTrackingBehavior.TrackAll);

        private ITenantsDatabaseContext CreateContext(QueryTrackingBehavior trackingBehavior)
        {
            var context = new TenantsDatabaseContext(options);
            context.ChangeTracker.QueryTrackingBehavior = trackingBehavior;

            return context;
        }

        private DbContextOptions<TenantsDatabaseContext> CreateContextOptions(DatabaseOptions databaseOptions)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantsDatabaseContext>();
            optionsBuilder.UseSqlServer(databaseOptions.ConnectionString);

            return optionsBuilder.Options;
        }

        private readonly DbContextOptions<TenantsDatabaseContext> options;
    }
}
