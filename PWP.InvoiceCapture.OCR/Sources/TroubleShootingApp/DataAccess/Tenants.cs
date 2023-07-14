using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Utilities;
using System.Collections.Generic;
using TroubleShootingApp.Contracts;
using TroubleShootingApp.Models;
using TroubleShootingApp.Options;

namespace TroubleShootingApp.DataAccess
{
    public class TenantsRepository : RepositoryBase<Tenant>, ITenantRepository
    {
        public TenantsRepository(IOptions<TenantDbOptions> options)
        {
            Guard.IsNotNull(options, nameof(options));
            Guard.IsNotNullOrEmpty(options.Value.ConnectionString, nameof(options.Value.ConnectionString));
            cnString = options.Value.ConnectionString;
        }

        public List<Tenant> GetAll()
        {
            string sql = "Select * from Tenant";
            return GetAll(sql,cnString,(newobj)=> { return new Tenant { Name = newobj.Name, DatabaseName = newobj.DatabaseName };  });       
        }

        private readonly string cnString;
    }
}
