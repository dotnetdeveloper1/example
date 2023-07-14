using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts
{
    internal interface IInvoiceProcessingResultRepository
    {
        Task<List<InvoiceProcessingResult>> GetCompletedListAsync(CancellationToken cancellationToken);
    }
}
