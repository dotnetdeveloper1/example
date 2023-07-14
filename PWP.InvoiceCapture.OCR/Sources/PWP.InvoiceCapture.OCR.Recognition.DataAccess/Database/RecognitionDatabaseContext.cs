using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Configurations;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Configurations;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Database
{
    internal class RecognitionDatabaseContext : DbContext, IRecognitionDatabaseContext
    {
        public RecognitionDatabaseContext(DbContextOptions<RecognitionDatabaseContext> options) : base(options)
        { }

        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
            base.SaveChangesAsync(cancellationToken);

        public DbSet<InvoiceTemplate> InvoiceTemplates { get; set; }
        public DbSet<GeometricFeatureCollection> GeometricFeatures { get; set; }
        public DbSet<LabelOfInterest> LabelsOfInterest { get; set; }
        public DbSet<LabelKeyWord> LabelKeyWords { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            InvoiceTemplateConfiguration.Configure(modelBuilder.Entity<InvoiceTemplate>());
            GeometricFeaturesConfiguration.Configure(modelBuilder.Entity<GeometricFeatureCollection>());
            LabelsOfInterestConfiguration.Configure(modelBuilder.Entity<LabelOfInterest>());
            KeywordsConfiguration.Configure(modelBuilder.Entity<LabelKeyWord>());
            SynonymsConfiguration.Configure(modelBuilder.Entity<LabelSynonym>());
        }
    }
}
