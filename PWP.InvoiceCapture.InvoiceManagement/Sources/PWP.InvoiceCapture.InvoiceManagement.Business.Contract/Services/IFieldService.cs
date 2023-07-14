using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IFieldService
    {
        Dictionary<string, int> GetFieldTypes();
        Dictionary<string, int> GetTargetFieldTypes();
        Task<Field> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<Field>> GetListAsync(CancellationToken cancellationToken);
        Task<OperationResult> CreateAsync(Field field, CancellationToken cancellationToken);
        Task<OperationResult> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<OperationResult> UpdateAsync(int fieldId, Field field, CancellationToken cancellationToken);
    }
}
