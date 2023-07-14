using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories
{
    public interface IFieldRepository
    {
        Task<Field> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<Field>> GetListAsync(CancellationToken cancellationToken);
        Task CreateAsync(Field field, CancellationToken cancellationToken);
        Task UpdateAsync(int fieldId, Field field, CancellationToken cancellationToken);
        Task DeleteAsync(int fieldId, CancellationToken cancellationToken);
        Task DeleteAsync(List<int> fieldIds, CancellationToken cancellationToken);
    }
}
