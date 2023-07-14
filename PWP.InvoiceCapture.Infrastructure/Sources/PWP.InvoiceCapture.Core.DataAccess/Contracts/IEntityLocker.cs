using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.DataAccess.Contracts
{
    public interface IEntityLocker
    {
        Task LockAsync(CancellationToken cancellationToken);
        Task LockAsync(int id, CancellationToken cancellationToken);
    }
}
