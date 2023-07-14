using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories
{
    public interface IInvoiceTemplateCultureSettingRepository
    {
        Task CreateAsync(InvoiceTemplateCultureSetting invoiceTemplateCultureSetting, CancellationToken cancellationToken);
        Task<InvoiceTemplateCultureSetting> GetByTemplateIdAsync(string templateId, CancellationToken cancellationToken);
        Task UpdateAsync(InvoiceTemplateCultureSetting invoiceTemplateCultureSetting, CancellationToken cancellationToken);
    }
}
