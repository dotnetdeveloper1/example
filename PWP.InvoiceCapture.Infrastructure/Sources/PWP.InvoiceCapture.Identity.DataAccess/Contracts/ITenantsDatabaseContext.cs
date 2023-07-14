using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Contracts
{
    internal interface ITenantsDatabaseContext : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DatabaseFacade Database { get; }

        DbSet<Tenant> Tenants { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Plan> Plans { get; set; }
        DbSet<Pack> Packs { get; set; }
        DbSet<Currency> Currencies { get; set; }
        DbSet<ApplicationClient> ApplicationClients { get; set; }
        DbSet<Group> Groups { get; set; }
        DbSet<GroupPlan> GroupPlans { get; set; }
        DbSet<GroupPack> GroupPacks { get; set; }
        DbSet<Culture> Cultures { get; set; }
        DbSet<TenantSetting> TenantSettings { get; set; }
        DbSet<PersistedGrant> PersistedGrants { get; set; }
    }
}
