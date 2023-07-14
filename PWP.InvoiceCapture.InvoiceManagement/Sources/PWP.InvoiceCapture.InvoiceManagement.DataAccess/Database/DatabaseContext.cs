using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Configurations;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database
{
    internal class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
            base.SaveChangesAsync(cancellationToken);

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoicePage> InvoicePages { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<InvoiceProcessingResult> InvoiceProcessingResults { get; set; }
        public DbSet<InvoiceTemplateCultureSetting> InvoiceTemplateCultureSettings { get; set; }
        public DbSet<InvoiceField> InvoiceFields { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldGroup> FieldGroups { get; set; }
        public DbSet<FormulaField> FormulaFields { get; set; }
        public DbSet<Webhook> Webhooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            InvoiceConfiguration.Configure(modelBuilder.Entity<Invoice>());
            InvoicePageConfiguration.Configure(modelBuilder.Entity<InvoicePage>());
            InvoiceLineConfiguration.Configure(modelBuilder.Entity<InvoiceLine>());
            InvoiceTemplateCultureSettingConfiguration.Configure(modelBuilder.Entity<InvoiceTemplateCultureSetting>());
            InvoiceProcessingResultConfiguration.Configure(modelBuilder.Entity<InvoiceProcessingResult>());
            FieldConfiguration.Configure(modelBuilder.Entity<Field>());
            FormulaFieldConfiguration.Configure(modelBuilder.Entity<FormulaField>());
            FieldGroupConfiguration.Configure(modelBuilder.Entity<FieldGroup>());
            InvoiceFieldConfiguration.Configure(modelBuilder.Entity<InvoiceField>());
            WebhookConfiguration.Configure(modelBuilder.Entity<Webhook>());
        }
    }
}
