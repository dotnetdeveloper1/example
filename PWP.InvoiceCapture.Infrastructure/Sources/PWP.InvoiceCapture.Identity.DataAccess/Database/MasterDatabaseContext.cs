using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.DataAccess.Configurations;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Database
{
    internal class MasterDatabaseContext : DbContext, IMasterDatabaseContext
    {
        public MasterDatabaseContext(DbContextOptions<MasterDatabaseContext> options) : base(options)
        { }

        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
            base.SaveChangesAsync(cancellationToken);

        public DbSet<SqlDatabase> Databases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SqlDatabaseConfiguration.Configure(modelBuilder.Entity<SqlDatabase>(), Database.IsSqlServer());
        }
    }
}
