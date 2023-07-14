using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TroubleShootingApp.Contracts
{
    public interface IFieldsRepository
    {
        Task<List<Field>> GetNotDeletedAsync(string tenantId);
    }
}
