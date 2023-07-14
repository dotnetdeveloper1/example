using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Configurations;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Database
{
    internal class InvoicesDatabaseContext : DbContext
    {
        public InvoicesDatabaseContext(DbContextOptions<InvoicesDatabaseContext> options) : base(options)
        { }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceProcessingResult> InvoiceProcessingResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            InvoiceConfiguration.Configure(modelBuilder.Entity<Invoice>());
            InvoiceProcessingResultConfiguration.Configure(modelBuilder.Entity<InvoiceProcessingResult>());
        }
    }
}
