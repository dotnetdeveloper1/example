using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Contracts
{
    internal interface IMasterDatabaseContext : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<SqlDatabase> Databases { get; set; }
    }
}
