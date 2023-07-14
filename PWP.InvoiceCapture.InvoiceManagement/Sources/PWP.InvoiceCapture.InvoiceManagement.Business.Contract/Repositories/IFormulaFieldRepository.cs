using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories
{
    public interface IFormulaFieldRepository
    {
        Task<List<FormulaField>> GetByResultFieldIdAsync(int fieldId, CancellationToken cancellationToken);
        Task<List<FormulaField>> GetByOperandFieldIdAsync(int fieldId, CancellationToken cancellationToken);
        Task CreateAsync(int fieldId, List<int> operandIds, CancellationToken cancellationToken);
        Task CreateAsync(int fieldId, int operandId, CancellationToken cancellationToken);
        Task DeleteAllByResultFieldIdAsync(int fieldId, CancellationToken cancellationToken);
        Task<bool> UsedAsOperandInFormulaAsync(int fieldId, CancellationToken cancellationToken);
        Task<bool> UsedAsResultFieldInFormulaAsync(int fieldId, CancellationToken cancellationToken);
    }
}
