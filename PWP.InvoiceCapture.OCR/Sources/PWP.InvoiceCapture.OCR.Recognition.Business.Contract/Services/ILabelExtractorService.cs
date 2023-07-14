using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface ILabelExtractorService
    {
        Task<Dictionary<LabelOfInterest, List<WordGroup>>> ExtractLabelsAsync(OCRElements ocrElements,CancellationToken cancellationToken);

        Task<Dictionary<LabelOfInterest, List<WordGroup>>> ExtractLabelsFromPairsAsync(OCRElements ocrElements, CancellationToken cancellationToken);

        Task<Dictionary<LabelOfInterest, List<WordGroup>>> ExtractLabelsFromNGramsAsync(OCRElements ocrElements, CancellationToken cancellationToken);

    }
}
