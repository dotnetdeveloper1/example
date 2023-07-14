using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Mappers;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class InvoiceProcessingResultService : IInvoiceProcessingResultService
    {
        public InvoiceProcessingResultService(
            IInvoiceProcessingResultRepository processingResultRepository,
            IInvoiceTemplateCultureSettingRepository invoiceTemplateCultureSettingRepository,
            IInvoiceService invoiceService,
            IInvoiceLineService invoiceLineService,
            IInvoiceFieldService invoiceFieldService,
            IAnnotationMapper annotationMapper,
            IDocumentApiClient documentApiClient,
            ISerializationService serializationService,
            IDataAnnotationValidator dataAnnotationValidator,
            IServiceBusPublisher publisher,
            IApplicationContext applicationContext,
            IFieldService fieldService)
        {
            Guard.IsNotNull(processingResultRepository, nameof(processingResultRepository));
            Guard.IsNotNull(invoiceTemplateCultureSettingRepository, nameof(invoiceTemplateCultureSettingRepository));
            Guard.IsNotNull(invoiceService, nameof(invoiceService));
            Guard.IsNotNull(invoiceLineService, nameof(invoiceLineService));
            Guard.IsNotNull(invoiceFieldService, nameof(invoiceFieldService));
            Guard.IsNotNull(annotationMapper, nameof(annotationMapper));
            Guard.IsNotNull(documentApiClient, nameof(documentApiClient));
            Guard.IsNotNull(serializationService, nameof(serializationService));
            Guard.IsNotNull(dataAnnotationValidator, nameof(dataAnnotationValidator));
            Guard.IsNotNull(publisher, nameof(publisher));
            Guard.IsNotNull(applicationContext, nameof(applicationContext));
            Guard.IsNotNull(fieldService, nameof(fieldService));

            this.processingResultRepository = processingResultRepository;
            this.invoiceTemplateCultureSettingRepository = invoiceTemplateCultureSettingRepository;
            this.invoiceService = invoiceService;
            this.invoiceLineService = invoiceLineService;
            this.invoiceFieldService = invoiceFieldService;
            this.annotationMapper = annotationMapper;
            this.documentApiClient = documentApiClient;
            this.serializationService = serializationService;
            this.dataAnnotationValidator = dataAnnotationValidator;
            this.publisher = publisher;
            this.applicationContext = applicationContext;
            this.fieldService = fieldService;
        }

        public async Task<InvoiceProcessingResult> GetAsync(int processingResultId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(processingResultId, nameof(processingResultId));

            var processingResult = await processingResultRepository.GetAsync(processingResultId, cancellationToken);

            if (processingResult != null)
            {
                await SetDataAnnotationAsync(processingResult, cancellationToken);
                await SetVendorNameAsync(processingResult, cancellationToken);
                await SetCultureToProcessingResultAsync(processingResult, cancellationToken);
            }

            return processingResult;
        }

        private async Task SetCultureToProcessingResultAsync(InvoiceProcessingResult processingResult, CancellationToken cancellationToken)
        {
            var culture = await invoiceTemplateCultureSettingRepository.GetByTemplateIdAsync(processingResult.TemplateId, cancellationToken);

            processingResult.CultureName = culture == null ? applicationContext.Culture : culture.CultureName;
        }

        public async Task<List<InvoiceProcessingResult>> GetListAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            var processingResults = await processingResultRepository.GetByInvoiceIdAsync(invoiceId, cancellationToken);

            foreach (var procesingResult in processingResults)
            {
                await SetDataAnnotationAsync(procesingResult, cancellationToken);
                await SetCultureToProcessingResultAsync(procesingResult, cancellationToken);
                await SetVendorNameAsync(procesingResult, cancellationToken);
            }

            return processingResults;
        }

        public async Task<InvoiceProcessingResult> GetLatestAsync(int invoiceId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            var lastProcessingResult = await processingResultRepository.GetLastByInvoiceIdAsync(invoiceId, cancellationToken);

            if (lastProcessingResult != null)
            {
                await SetDataAnnotationAsync(lastProcessingResult, cancellationToken);
                await SetVendorNameAsync(lastProcessingResult, cancellationToken);
                await SetCultureToProcessingResultAsync(lastProcessingResult, cancellationToken);
            }

            return lastProcessingResult;
        }

        public async Task CreateAsync(InvoiceProcessingResult processingResult, CancellationToken cancellationToken)
        {
            GuardProcessingResult(processingResult);

            using (var transaction = TransactionManager.Create())
            {
                await processingResultRepository.CreateAsync(processingResult, cancellationToken);
                await invoiceService.UpdateStatusAsync(processingResult.InvoiceId, InvoiceStatus.PendingReview, cancellationToken);

                transaction.Complete();
            }
        }

        public async Task<OperationResult> UpdateDataAnnotationAsync(int processingResultId, UpdatedDataAnnotation updatedDataAnnotation, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(processingResultId, nameof(processingResultId));
            Guard.IsNotNull(updatedDataAnnotation, nameof(updatedDataAnnotation));
            Guard.IsNotNull(updatedDataAnnotation.DataAnnotation, nameof(updatedDataAnnotation.DataAnnotation));

            using (var transaction = TransactionManager.Create())
            {
                var invoiceId = await processingResultRepository.GetInvoiceIdAsync(processingResultId, cancellationToken);

                if (invoiceId == null)
                {
                    return new OperationResult { Status = OperationResultStatus.NotFound, Message = $"Processing result with id = {processingResultId} was not found." };
                }

                // Aquire exclusive row lock on Processing Result to be sure other transactions cannot modify it till this transaction ends.
                await processingResultRepository.LockAsync(processingResultId, cancellationToken);
                await invoiceService.LockAsync(invoiceId.Value, cancellationToken);


                var processingResult = await processingResultRepository.GetAsync(processingResultId, cancellationToken);

                if (processingResult.Invoice.Status != InvoiceStatus.PendingReview)
                {
                    return new OperationResult { Status = OperationResultStatus.Failed, Message = "Invoice is not in status PendingReview." };
                }
                
                CultureInfo cultureInfo;
                if (string.IsNullOrWhiteSpace(updatedDataAnnotation.CultureName))
                {
                    cultureInfo = new CultureInfo(applicationContext.Culture);
                }
                else
                {
                    cultureInfo = new CultureInfo(updatedDataAnnotation.CultureName);
                    await UpdateOrCreateInvoiceTemplateCultureSettingsAsync(processingResult.TemplateId, updatedDataAnnotation.CultureName, cancellationToken);
                }

                var fields = await fieldService.GetListAsync(cancellationToken);

                var dataAnnotation = updatedDataAnnotation.DataAnnotation;
                var dataAnnotationValidationResult = dataAnnotationValidator.Validate(dataAnnotation, cultureInfo, fields);

                if (!dataAnnotationValidationResult.IsValid)
                {
                    return new OperationResult { Status = OperationResultStatus.Failed, Message = dataAnnotationValidationResult.Message };
                }

                await RemoveValidationMessageAsync(invoiceId.Value, cancellationToken);

                var updateInvoiceFieldsTask = UpdateInvoiceFieldsAsync(processingResult.InvoiceId, dataAnnotation.InvoiceAnnotations, fields, cultureInfo, cancellationToken);
                var updateInvoiceLinesTask = UpdateInvoiceLinesAsync(processingResult.InvoiceId, dataAnnotation.InvoiceLineAnnotations, cultureInfo, cancellationToken);
                
                var fileName = await UploadDataAnnotationAsync(dataAnnotation, cancellationToken);

                await Task.WhenAll(
                    processingResultRepository.UpdateDataAnnotationFileIdAsync(processingResultId, fileName, cancellationToken),
                    updateInvoiceFieldsTask,
                    updateInvoiceLinesTask);

                transaction.Complete();
            }

            return OperationResult.Success;
        }

        public async Task<OperationResult> CompleteAsync(int processingResultId, UpdatedDataAnnotation updatedDataAnnotation, CancellationToken cancellationToken)
        {
            var updateResult = await UpdateDataAnnotationAsync(processingResultId, updatedDataAnnotation, cancellationToken);

            if (!updateResult.IsSuccessful)
            {
                return updateResult;
            }

            InvoiceProcessingResult processingResult = null;

            using (var transaction = TransactionManager.Create())
            {
                var invoiceId = await processingResultRepository.GetInvoiceIdAsync(processingResultId, cancellationToken);

                // Aquire exclusive row lock on Processing Result to be sure other transactions cannot modify it till this transaction ends.
                await processingResultRepository.LockAsync(processingResultId, cancellationToken);
                await invoiceService.LockAsync(invoiceId.Value, cancellationToken);

                processingResult = await processingResultRepository.GetAsync(processingResultId, cancellationToken);

                if (processingResult.Invoice.Status != InvoiceStatus.PendingReview)
                {
                    return new OperationResult { Status = OperationResultStatus.Failed, Message = "Invoice is not in status PendingReview." };
                }

                await invoiceService.UpdateStatusAsync(processingResult.InvoiceId, InvoiceStatus.Completed, cancellationToken);
                await invoiceService.UpdateStateAsync(processingResult.InvoiceId, InvoiceState.Locked, cancellationToken);

                transaction.Complete();
            }

            await PublishInvoiceCompletedMessageAsync(processingResult, cancellationToken);

            return OperationResult.Success;
        }

        private async Task RemoveValidationMessageAsync(int invoiceId, CancellationToken cancellationToken)
        {
            await invoiceService.UpdateValidationMessageAsync(invoiceId, null, cancellationToken);
        }

        public async Task ValidateCreatedInvoiceAsync(int invoiceId, string cultureName, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(invoiceId, nameof(invoiceId));

            var processingResult = await processingResultRepository.GetLastByInvoiceIdAsync(invoiceId, cancellationToken);
            var currentCulture = await GetCurrentCultureForTemplateAsync(processingResult.TemplateId, cultureName, cancellationToken);

            await SetDataAnnotationAsync(processingResult, cancellationToken);

            var fields = await fieldService.GetListAsync(cancellationToken);
            
            var validationResult = dataAnnotationValidator.Validate(processingResult.DataAnnotation, currentCulture, fields);

            if (validationResult.IsValid)
            {
                return;
            }
            await invoiceService.UpdateValidationMessageAsync(invoiceId, validationResult.Message, cancellationToken);
        }

        private async Task PublishInvoiceCompletedMessageAsync(InvoiceProcessingResult processingResult, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(processingResult, nameof(processingResult));
            Guard.IsNotNull(processingResult.Invoice, nameof(processingResult.Invoice));

            var message = new InvoiceCompletedMessage
            {
                InvoiceId = processingResult.InvoiceId,
                TemplateId = processingResult.TemplateId,
                InvoiceFileId = processingResult.Invoice.FileId,
                DataAnnotationFileId = processingResult.DataAnnotationFileId,
                TenantId = applicationContext.TenantId
            };

            await publisher.PublishAsync(message, cancellationToken);
        }

        private async Task UpdateOrCreateInvoiceTemplateCultureSettingsAsync(string templateId, string cultureName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(templateId, nameof(templateId));
            Guard.IsNotNullOrWhiteSpace(cultureName, nameof(cultureName));

            var existingCultureSetting = await invoiceTemplateCultureSettingRepository.GetByTemplateIdAsync(templateId, cancellationToken);
            if (existingCultureSetting == null)
            {
                await invoiceTemplateCultureSettingRepository.CreateAsync(new InvoiceTemplateCultureSetting { CultureName = cultureName, TemplateId = templateId }, cancellationToken);
                return;
            }
            existingCultureSetting.CultureName = cultureName;

            await invoiceTemplateCultureSettingRepository.UpdateAsync(existingCultureSetting, cancellationToken);
        }

        private async Task<string> UploadDataAnnotationAsync(DataAnnotation dataAnnotation, CancellationToken cancellationToken)
        {
            var serializedAnnotations = serializationService.Serialize(dataAnnotation);
            var uploadedFileName = "dataAnnotation.json";

            using (var contentToUpload = new MemoryStream(Encoding.UTF8.GetBytes(serializedAnnotations)))
            {
                var response = await documentApiClient.UploadDocumentAsync(contentToUpload, uploadedFileName, cancellationToken);
                uploadedFileName = response.Data.FileId;
            }

            return uploadedFileName;
        }

        private async Task UpdateInvoiceFieldsAsync(int invoiceId, List<Annotation> annotations, List<Field> fields, CultureInfo cultureInfo, CancellationToken cancellationToken)
        {
            var existingInvoiceFields = await invoiceFieldService.GetListAsync(invoiceId, cancellationToken);

            var newInvoiceFields = annotations
                .Where(annotation => !existingInvoiceFields
                    .Any(invoiceField => Equals(annotation.FieldType, invoiceField.FieldId.ToString())))
                .Select(annotation => annotationMapper.ToInvoiceField(invoiceId, annotation, fields, cultureInfo))
                .ToList();

            var updateInvoiceFields = annotations
                .Where(annotation => existingInvoiceFields
                    .Any(invoiceField => Equals(annotation.FieldType, invoiceField.FieldId.ToString())))
                .Select(annotation => {
                    var existingInvoiceField = existingInvoiceFields
                        .First(invoiceField => invoiceField.FieldId.ToString() == annotation.FieldType);
                    existingInvoiceField.Value = annotation.FieldValue;                    
                    return existingInvoiceField;
                })
                .ToList();

            if (newInvoiceFields.Any())
            {
                await invoiceFieldService.CreateAsync(newInvoiceFields, cancellationToken);
            }
            if (updateInvoiceFields.Any())
            {
                await invoiceFieldService.UpdateAsync(updateInvoiceFields, cancellationToken);
            }
        }

        private async Task UpdateInvoiceLinesAsync(int invoiceId, List<LineAnnotation> lineAnnotations, CultureInfo cultureInfo, CancellationToken cancellationToken)
        {
            await invoiceLineService.DeleteByInvoiceIdAsync(invoiceId, cancellationToken);

            if (lineAnnotations == null || lineAnnotations.Count == 0)
            {
                return;
            }

            var invoiceLines = lineAnnotations.Select(annotation => annotationMapper.ToInvoiceLine(annotation, invoiceId, cultureInfo)).ToList();

            await invoiceLineService.CreateAsync(invoiceLines, cancellationToken);
        }

        private void GuardProcessingResult(InvoiceProcessingResult processingResult)
        {
            Guard.IsNotNull(processingResult, nameof(processingResult));
            Guard.IsNotZeroOrNegative(processingResult.InvoiceId, nameof(processingResult.InvoiceId));
            Guard.IsNotNullOrWhiteSpace(processingResult.TemplateId, nameof(processingResult.TemplateId));
            Guard.IsNotNullOrWhiteSpace(processingResult.DataAnnotationFileId, nameof(processingResult.DataAnnotationFileId));
        }

        private async Task SetDataAnnotationAsync(InvoiceProcessingResult processingResult, CancellationToken cancellationToken)
        {
            using (var dataAnnotationsStream = await documentApiClient.GetDocumentStreamAsync(processingResult.DataAnnotationFileId, cancellationToken))
            using (var streamReader = new StreamReader(dataAnnotationsStream))
            {
                var json = await streamReader.ReadToEndAsync();
                processingResult.DataAnnotation = serializationService.Deserialize<DataAnnotation>(json);
            }
        }

        private async Task SetVendorNameAsync(InvoiceProcessingResult processingResult, CancellationToken cancellationToken)
        {
            var vendorName = await processingResultRepository.GetVendorNameByTemplateIdAsync(processingResult.TemplateId, cancellationToken);
            processingResult.VendorName = vendorName;
        }

        private async Task<CultureInfo> GetCurrentCultureForTemplateAsync(string templateId, string cultureNameFromMessage, CancellationToken cancellationToken)
        {
            var culture = await invoiceTemplateCultureSettingRepository.GetByTemplateIdAsync(templateId, cancellationToken);
           
            var cultureInfo = culture == null ? new CultureInfo(cultureNameFromMessage) : new CultureInfo(culture.CultureName);

            return cultureInfo;
        }

        private readonly IInvoiceProcessingResultRepository processingResultRepository;
        private readonly IInvoiceTemplateCultureSettingRepository invoiceTemplateCultureSettingRepository;
        private readonly IInvoiceService invoiceService;
        private readonly IInvoiceLineService invoiceLineService;
        private readonly IInvoiceFieldService invoiceFieldService;
        private readonly IFieldService fieldService;
        private readonly IAnnotationMapper annotationMapper;
        private readonly IDocumentApiClient documentApiClient;
        private readonly ISerializationService serializationService;
        private readonly IDataAnnotationValidator dataAnnotationValidator;
        private readonly IServiceBusPublisher publisher;
        private readonly IApplicationContext applicationContext;
    }
}
