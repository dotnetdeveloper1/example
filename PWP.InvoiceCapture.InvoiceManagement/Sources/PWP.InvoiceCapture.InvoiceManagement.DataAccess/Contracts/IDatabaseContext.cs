using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts
{
    internal interface IDatabaseContext : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DatabaseFacade Database { get; }

        DbSet<Invoice> Invoices { get; set; }
        DbSet<InvoicePage> InvoicePages { get; set; }
        DbSet<InvoiceLine> InvoiceLines { get; set; }        
        DbSet<InvoiceProcessingResult> InvoiceProcessingResults { get; set; }
        DbSet<InvoiceTemplateCultureSetting> InvoiceTemplateCultureSettings { get; set; }
        DbSet<InvoiceField> InvoiceFields { get; set; }
        DbSet<FormulaField> FormulaFields { get; set; }
        DbSet<Field> Fields { get; set; }
        DbSet<FieldGroup> FieldGroups { get; set; }
        DbSet<Webhook> Webhooks { get; set; }
    }
}
