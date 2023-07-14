using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services
{
    public interface IRecognitionEngine
    {
        Task<OperationResult<RecognitionEngineResponse>> ProcessDocumentAsync(List<FieldTargetField> fieldTargetFields, int invoiceId, string fileId, string imageFileId, string tenantId, CancellationToken cancellationToken);
    }
}
