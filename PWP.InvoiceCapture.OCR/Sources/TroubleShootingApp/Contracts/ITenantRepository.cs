using System.Collections.Generic;
using TroubleShootingApp.Models;

namespace TroubleShootingApp.Contracts
{
    public interface ITenantRepository
    {
        List<Tenant> GetAll();
    }
}
