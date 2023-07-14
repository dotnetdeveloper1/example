using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.DataAccess.Configurations;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Database
{
    internal class TenantsDatabaseContext : DbContext, ITenantsDatabaseContext
    {
        public TenantsDatabaseContext(DbContextOptions<TenantsDatabaseContext> options) : base(options)
        { }

        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
            base.SaveChangesAsync(cancellationToken);

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ApplicationClient> ApplicationClients { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Pack> Packs { get; set; }
        public DbSet<Culture> Cultures { get; set; }
        public DbSet<TenantSetting> TenantSettings { get; set; }
        public DbSet<GroupPlan> GroupPlans { get; set; }
        public DbSet<GroupPack> GroupPacks { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            UserConfiguration.Configure(modelBuilder.Entity<User>());
            ApplicationClientConfiguration.Configure(modelBuilder.Entity<ApplicationClient>());
            TenantConfiguration.Configure(modelBuilder.Entity<Tenant>());
            GroupsConfiguration.Configure(modelBuilder.Entity<Group>());
            PlanConfiguration.Configure(modelBuilder.Entity<Plan>());
            PackConfiguration.Configure(modelBuilder.Entity<Pack>());
            CulturesConfiguration.Configure(modelBuilder.Entity<Culture>());
            TenantSettingsConfiguration.Configure(modelBuilder.Entity<TenantSetting>());
            CurrencyConfiguration.Configure(modelBuilder.Entity<Currency>());
            GroupPlanConfiguration.Configure(modelBuilder.Entity<GroupPlan>());
            GroupPackConfiguration.Configure(modelBuilder.Entity<GroupPack>());
            PersistedGrantConfiguration.Configure(modelBuilder.Entity<PersistedGrant>());
        }
    }
}
