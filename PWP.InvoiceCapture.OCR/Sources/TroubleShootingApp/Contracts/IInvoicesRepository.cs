using System.Collections.Generic;
using TroubleShootingApp.Models;

namespace TroubleShootingApp.Contracts
{
    public interface IInvoicesRepository
    {
        List<Invoice> GetAll(string tenantId);
    }
}
