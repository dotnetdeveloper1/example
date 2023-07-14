using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface IPostOcrTemplateMatchingService
    {
        Task<Dictionary<string, Coordinate>> GetKeyWordCoordinatesAsync(OCRElements ocrElements, CancellationToken cancellationToken);
        InvoiceTemplate GetMatchingTemplate(Dictionary<string, Coordinate> keyWordCoordinates, List<InvoiceTemplate> invoiceTemplates);
        int MinKeywordsCount { get; }
    }
}
