using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using System.Collections.Generic;
using TroubleShootingApp.Contracts;
using TroubleShootingApp.Models;
using TroubleShootingApp.Options;

namespace TroubleShootingApp.DataAccess
{
    public class InvoiceRepository : RepositoryBase<Invoice>, IInvoicesRepository
    {
        public InvoiceRepository(IInvoicesDatabaseNameProvider invoicesDatabaseNameProvider, IOptions<InvoiceDbOptions> options)
        {
            Guard.IsNotNull(invoicesDatabaseNameProvider, nameof(invoicesDatabaseNameProvider));
            Guard.IsNotNull(options, nameof(options));
            Guard.IsNotNullOrEmpty(options.Value.ConnectionString, nameof(options.Value.ConnectionString));
            this.invoicesDatabaseNameProvider = invoicesDatabaseNameProvider;
            this.cnStringTemplate = options.Value.ConnectionString;
        }

        public List<Invoice> GetAll(string tenantId)
        {
            string sql = "SELECT * FROM[dbo].[Invoice] i Inner join[dbo].[InvoiceProcessingResult] ipr on i.Id = ipr.InvoiceId";
            string dbName = invoicesDatabaseNameProvider.Get(tenantId);
            string cnString = string.Format(cnStringTemplate, dbName);
            return base.GetAll(sql, cnString, (newobj) => { return new Invoice { Name = newobj.Name, FileId = newobj.FileId, FileName = newobj.FileName, StatusId = newobj.StatusId, TemplateId = newobj.TemplateId }; });
        }

        private readonly IInvoicesDatabaseNameProvider invoicesDatabaseNameProvider;
        private readonly string cnStringTemplate;
    }
}
