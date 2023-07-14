using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories
{
    public interface ILabelOfInterestRepository
    {
        Task<IEnumerable<LabelOfInterest>> GetAllAsync(CancellationToken cancellationToken);
    }
}
