using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PWP.InvoiceCapture.OCR.Core.Models; 
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess
{
    public interface IDatabaseContext : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DatabaseFacade Database { get; }

        DbSet<InvoiceTemplate> InvoiceTemplates { get; set; }
        DbSet<FormRecognizerResource> FormRecognizers { get; set; }
    }
}
