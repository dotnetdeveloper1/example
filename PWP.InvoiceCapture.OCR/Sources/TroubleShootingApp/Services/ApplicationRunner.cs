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
    public class ApplicationRunner : IApplicationRunner
    {
        public ApplicationRunner(IFormRecognizerClient frClient , IInvoiceTemplateRepository templateRepo , ILabelOfInterestRepository labelRepo,
            IDocumentApiClient documentApiClient, IRecognitionEngine recognitionEngine, ITrainingBlobRepository trainingBlobRepository, Contracts.ITenantRepository tenantRepo, IInvoicesRepository invoiceRepo, IFieldsRepository fieldRepo, IFieldMapper fieldMapper)
        {   
            this.frClient = frClient;
            this.templateRepo = templateRepo;
            this.labelRepo = labelRepo;
            this.documentApiClient = documentApiClient;
            this.recognitionEngine = recognitionEngine;
            this.trainingBlobRepository = trainingBlobRepository;
            this.tenantRepo = tenantRepo;
            this.invoiceRepo = invoiceRepo;
            this.fieldRepo = fieldRepo;
            this.fieldMapper = fieldMapper;
        }

        public IFormRecognizerClient GetFrClient()
        {
            return frClient;
        }
        

        public IInvoiceTemplateRepository GetInvoiceTemplateRepository()
        {
            return templateRepo;
        }

        public ILabelOfInterestRepository GetLabelRepository()
        {
            return labelRepo;
        }

        public IDocumentApiClient GetDocumentApiClient()
        {
            return documentApiClient;
        }

        public IRecognitionEngine GetRecognitionEngine()
        {
            return recognitionEngine;
        }

        public ITrainingBlobRepository GetTrainingBlobRepository()
        {
            return trainingBlobRepository;
        }

        public Contracts.ITenantRepository GetTenantsRepository()
        {
            return tenantRepo;
        }

        public IInvoicesRepository GetInvoicesRepository()
        {
            return invoiceRepo;
        }

        public IFieldsRepository GetFieldsRepository()
        {
            return fieldRepo;
        }

        public IFieldMapper GetFieldMapper()
        {
            return fieldMapper;
        }


        private readonly IFormRecognizerClient frClient;
        private readonly IInvoiceTemplateRepository templateRepo;
        private readonly ILabelOfInterestRepository labelRepo;
        private readonly IDocumentApiClient documentApiClient;
        private readonly IRecognitionEngine recognitionEngine;
        private readonly ITrainingBlobRepository trainingBlobRepository;
        private readonly Contracts.ITenantRepository tenantRepo;
        private readonly IInvoicesRepository invoiceRepo;
        private readonly IFieldsRepository fieldRepo;
        private readonly IFieldMapper fieldMapper;
    }
}
