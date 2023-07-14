using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.OCR.Core.DataAccess;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Configurations;
using PWP.InvoiceCapture.OCR.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Database
{
    internal class OCRDatabaseContext : DbContext, IDatabaseContext
    {
        public OCRDatabaseContext(DbContextOptions<OCRDatabaseContext> options) : base(options)
        { }

        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
            base.SaveChangesAsync(cancellationToken);

        public DbSet<InvoiceTemplate> InvoiceTemplates { get; set; }
        public DbSet<GeometricFeatureCollection> GeometricFeatures { get; set; }
        public DbSet<FormRecognizerResource> FormRecognizers { get; set; }
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            InvoiceTemplateConfiguration.Configure(modelBuilder.Entity<InvoiceTemplate>());
            GeometricFeaturesConfiguration.Configure(modelBuilder.Entity<GeometricFeatureCollection>());
            FormRecognizerResourceConfiguration.Configure(modelBuilder.Entity<FormRecognizerResource>());
        }
    }
}
