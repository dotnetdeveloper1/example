using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories
{
    public interface IFieldGroupRepository
    {
        Task<FieldGroup> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<FieldGroup>> GetListAsync(CancellationToken cancellationToken);
        Task CreateAsync(FieldGroup fieldGroup, CancellationToken cancellationToken);
        Task UpdateAsync(int fieldGroupId, FieldGroup fieldGroup, CancellationToken cancellationToken);
        Task DeleteAsync(int fieldGroupId, CancellationToken cancellationToken);
    }
}
