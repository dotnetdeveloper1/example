using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services
{
    public interface IFieldGroupService
    {
        Task<FieldGroup> GetAsync(int fieldGroupId, CancellationToken cancellationToken);
        Task<List<FieldGroup>> GetListAsync(CancellationToken cancellationToken);
        Task CreateAsync(FieldGroup fieldGroup, CancellationToken cancellationToken);
        Task<OperationResult> DeleteAsync(int fieldGroupId, CancellationToken cancellationToken);
        Task<OperationResult> DeleteFieldsByGroupIdAsync(int groupId, CancellationToken cancellationToken);
        Task<OperationResult> UpdateAsync(int fieldGroupId, FieldGroup fieldGroup, CancellationToken cancellationToken);
    }
}
