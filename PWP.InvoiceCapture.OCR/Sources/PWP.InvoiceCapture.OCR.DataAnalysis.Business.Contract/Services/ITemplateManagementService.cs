using PWP.InvoiceCapture.OCR.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Services
{
    public interface ITemplateManagementService
    {
        Task ProcessUserValidationDataAsync(int invoiceId, string fileId, string annotationFileId, int templateId, string tenantId, CancellationToken cancellationToken);
        Task<int> GetTemplateTrainingsCountAsync(int templateId, CancellationToken cancellationToken);
        Task<InvoiceTemplate> GetTemplateAsync(int templateId, CancellationToken cancellationToken);
    }
}
