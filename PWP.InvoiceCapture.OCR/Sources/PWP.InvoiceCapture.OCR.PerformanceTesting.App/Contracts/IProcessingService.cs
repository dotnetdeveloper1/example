using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts
{
    internal interface IProcessingService
    {
        Task ProcessAsync(CancellationToken cancellationToken);
    }
}
