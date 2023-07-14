using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.OCR.Core.DataAccess;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Contracts;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Database
{
    internal class OCRDatabaseContextFactory : IDatabaseContextFactory
    {
        public OCRDatabaseContextFactory(IOptions<DatabaseOptions> optionsAccessor) 
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(optionsAccessor.Value, nameof(optionsAccessor.Value));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.ConnectionString, nameof(optionsAccessor.Value.ConnectionString));
            options = CreateContextOptions(optionsAccessor.Value);
        }

        internal OCRDatabaseContextFactory(DbContextOptions<OCRDatabaseContext> options) 
        {
            Guard.IsNotNull(options, nameof(options));
            this.options = options;
        }

        public IDatabaseContext Create() => CreateContext(QueryTrackingBehavior.NoTracking);

        public IDatabaseContext CreateWithTracking() => CreateContext(QueryTrackingBehavior.TrackAll);

        private IDatabaseContext CreateContext(QueryTrackingBehavior trackingBehavior) 
        {
            var context = new OCRDatabaseContext(options);
            context.ChangeTracker.QueryTrackingBehavior = trackingBehavior;

            return context;
        }

        private DbContextOptions<OCRDatabaseContext> CreateContextOptions(DatabaseOptions databaseOptions) 
        {
            var optionsBuilder = new DbContextOptionsBuilder<OCRDatabaseContext>();
            optionsBuilder.UseSqlServer(databaseOptions.ConnectionString);

            return optionsBuilder.Options;
        }

        private readonly DbContextOptions<OCRDatabaseContext> options;
    }
}
