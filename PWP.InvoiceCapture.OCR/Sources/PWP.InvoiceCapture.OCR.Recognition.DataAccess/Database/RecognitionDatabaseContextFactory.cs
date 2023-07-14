using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Options;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Contracts;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Database
{
    internal class RecognitionDatabaseContextFactory : IRecognitionDatabaseContextFactory
    {
        public RecognitionDatabaseContextFactory(IOptions<RecognitionDatabaseOptions> optionsAccessor) 
        {
            Guard.IsNotNull(optionsAccessor, nameof(optionsAccessor));
            Guard.IsNotNull(optionsAccessor.Value, nameof(optionsAccessor.Value));
            Guard.IsNotNullOrWhiteSpace(optionsAccessor.Value.ConnectionString, nameof(optionsAccessor.Value.ConnectionString));
            options = CreateContextOptions(optionsAccessor.Value);
        }

        internal RecognitionDatabaseContextFactory(DbContextOptions<RecognitionDatabaseContext> options) 
        {
            Guard.IsNotNull(options, nameof(options));
            this.options = options;
        }

        public IRecognitionDatabaseContext Create() => CreateContext(QueryTrackingBehavior.NoTracking);

        public IRecognitionDatabaseContext CreateWithTracking() => CreateContext(QueryTrackingBehavior.TrackAll);

        private IRecognitionDatabaseContext CreateContext(QueryTrackingBehavior trackingBehavior) 
        {
            var context = new RecognitionDatabaseContext(options);
            context.ChangeTracker.QueryTrackingBehavior = trackingBehavior;

            return context;
        }

        private DbContextOptions<RecognitionDatabaseContext> CreateContextOptions(RecognitionDatabaseOptions databaseOptions) 
        {
            var optionsBuilder = new DbContextOptionsBuilder<RecognitionDatabaseContext>();
            optionsBuilder.UseSqlServer(databaseOptions.ConnectionString);

            return optionsBuilder.Options;
        }

        private readonly DbContextOptions<RecognitionDatabaseContext> options;
    }
}
