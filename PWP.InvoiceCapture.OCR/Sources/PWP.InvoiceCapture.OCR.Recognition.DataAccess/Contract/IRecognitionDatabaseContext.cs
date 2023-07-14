using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess
{
    public interface IRecognitionDatabaseContext : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DatabaseFacade Database { get; }

        DbSet<LabelOfInterest> LabelsOfInterest { get; set; }
        DbSet<Tenant> Tenants { get; set; }
    }
}
