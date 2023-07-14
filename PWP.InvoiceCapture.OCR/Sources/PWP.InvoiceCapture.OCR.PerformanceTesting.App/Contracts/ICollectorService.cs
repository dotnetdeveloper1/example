using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts
{
    internal interface ICollectorService
    {
        Task CollectAsync(CancellationToken cancellationToken);
    }
}
