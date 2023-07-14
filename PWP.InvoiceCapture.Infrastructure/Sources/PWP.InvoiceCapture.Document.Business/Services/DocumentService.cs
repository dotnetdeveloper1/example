using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using PWP.InvoiceCapture.Document.Business.Contract.Services;
using PWP.InvoiceCapture.Document.Business.Contract.Models;
using PWP.InvoiceCapture.Document.Business.Contract.Repositories;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Core.Models;

namespace PWP.InvoiceCapture.Document.Business.Services
{
    internal class DocumentService : IDocumentService
    {
        public DocumentService(IDocumentRepository documentRepository)
        {
            Guard.IsNotNull(documentRepository, nameof(documentRepository));

            this.documentRepository = documentRepository;
        }

        public async Task<string> CreateDocumentAsync(Stream fileContent, string fileExtension, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(fileContent, nameof(fileContent));

            var generatedDocumentName = Guid.NewGuid().ToString();
            var createDocumentArgs = new CreateDocumentArgs()
            {
                FileId = $"{generatedDocumentName}{fileExtension}",
                FileContent = fileContent
            };

            return await documentRepository.SaveAsync(createDocumentArgs, cancellationToken);
        }

        public async Task<OperationResult<string>> GetTemporaryLinkAsync(string fileId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(fileId, nameof(fileId));

            var result = await documentRepository.GetTemporaryLinkAsync(fileId, cancellationToken);

            return result == null
               ? OperationResult<string>.NotFound
               : OperationResult<string>.Success(result);
        }

        public async Task<OperationResult<GetDocumentStreamResult>> GetDocumentStreamAsync(string fileId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(fileId, nameof(fileId));

            var result = await documentRepository.GetStreamAsync(fileId, cancellationToken);

            return result == null
                ? OperationResult<GetDocumentStreamResult>.NotFound
                : OperationResult<GetDocumentStreamResult>.Success(result);
        }

        private readonly IDocumentRepository documentRepository;
    }
}
