using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.OCR.Core.Contract.Services;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Mappers;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using TroubleShootingApp.Contracts;

namespace TroubleShootingApp.Services
{
    public interface IApplicationRunner
    {
        IFormRecognizerClient GetFrClient();
        IInvoiceTemplateRepository GetInvoiceTemplateRepository();
        ILabelOfInterestRepository GetLabelRepository();
        IDocumentApiClient GetDocumentApiClient();
        IRecognitionEngine GetRecognitionEngine();

        ITrainingBlobRepository GetTrainingBlobRepository();
        Contracts.ITenantRepository GetTenantsRepository();
        IInvoicesRepository GetInvoicesRepository();
        IFieldsRepository GetFieldsRepository();
        IFieldMapper GetFieldMapper();
    }
}
