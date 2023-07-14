using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models;
using PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.Contract.Services
{
    public interface IFormRecognizerClient
    {
        int FormRecognizerId { get; }
        Task<TrainModelResponse> TrainModelAsync(string sasUri, CancellationToken cancellationToken);
        Task<FormRecognizerResponse> RunLayoutAnalysisAsync(string sasUri, CancellationToken cancellationToken);
        Task<FormRecognizerResponse> RunFormAnalysisAsync(string sasUri, string modelId, int formRecognizerId, CancellationToken cancellationToken);
        Task DeleteModelAsync(string modelId, int formRecognizerId, CancellationToken cancellationToken);
        Task<TrainModelResponse> GetModelDetailsAsync(string modelId, int formRecognizerId, CancellationToken cancellationToken);  
        Task<ListModelResponse> GetListModelResponseAsync(int formRecognizerId, CancellationToken cancellationToken);
        Task<AwaitModelReadinessResponse> AwaitModelReadinessAsync(string modelId, int formRecognizerId, CancellationToken cancellationToken);
    }
}
