using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.OCR.Core.Contract.Services;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories;
using PWP.InvoiceCapture.OCR.Core.Extensions;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Services
{
    public class RecognitionEngine : IRecognitionEngine
    {
        public RecognitionEngine(IFormRecognizerClient formRecognizerClient, IInvoiceAnalysisService invoiceAnalysisService,
            IRecognitionElementFactory recognitionElementFactory, IOCRElementsFactory oCRElementsFactory, IDataAnnotationsFactory dataAnnotationsFactory,
            IDocumentApiClient documentApiClient, IGeometricFeaturesService geometricFeaturesService, IInvoiceTemplateService invoiceTemplateService,
            IInvoiceTemplateRepository invoiceTemplateRepository, IOCRProviderRecognitionDataFactory ocrProviderRecognitionDataFactory,
            ITrainingBlobRepository trainingBlobRepository, IPostOcrTemplateMatchingService postOcrTemplateMatchingService, ITelemetryClient telemetryClient,
            ITenantTrackingService tenantTrackingService)
        {
            Guard.IsNotNull(formRecognizerClient, nameof(formRecognizerClient));
            Guard.IsNotNull(invoiceAnalysisService, nameof(invoiceAnalysisService));
            Guard.IsNotNull(recognitionElementFactory, nameof(recognitionElementFactory));
            Guard.IsNotNull(oCRElementsFactory, nameof(oCRElementsFactory));
            Guard.IsNotNull(dataAnnotationsFactory, nameof(dataAnnotationsFactory));
            Guard.IsNotNull(documentApiClient, nameof(documentApiClient));
            Guard.IsNotNull(geometricFeaturesService, nameof(geometricFeaturesService));
            Guard.IsNotNull(invoiceTemplateService, nameof(invoiceTemplateService));
            Guard.IsNotNull(invoiceTemplateRepository, nameof(invoiceTemplateRepository));
            Guard.IsNotNull(ocrProviderRecognitionDataFactory, nameof(ocrProviderRecognitionDataFactory));
            Guard.IsNotNull(trainingBlobRepository, nameof(trainingBlobRepository));
            Guard.IsNotNull(postOcrTemplateMatchingService, nameof(postOcrTemplateMatchingService));
            Guard.IsNotNull(telemetryClient, nameof(telemetryClient));
            Guard.IsNotNull(tenantTrackingService, nameof(tenantTrackingService));

            this.formRecognizerClient = formRecognizerClient;
            this.invoiceAnalysisService = invoiceAnalysisService;
            this.recognitionElementFactory = recognitionElementFactory;
            this.oCRElementsFactory = oCRElementsFactory;
            this.dataAnnotationsFactory = dataAnnotationsFactory;
            this.documentApiClient = documentApiClient;
            this.geometricFeaturesService = geometricFeaturesService;
            this.invoiceTemplateService = invoiceTemplateService;
            this.invoiceTemplateRepository = invoiceTemplateRepository;
            this.ocrProviderRecognitionDataFactory = ocrProviderRecognitionDataFactory;
            this.trainingBlobRepository = trainingBlobRepository;
            this.postOcrTemplateMatchingService = postOcrTemplateMatchingService;
            this.telemetryClient = telemetryClient;
            this.tenantTrackingService = tenantTrackingService;
        }

        public async Task<OperationResult<RecognitionEngineResponse>> ProcessDocumentAsync(List<FieldTargetField> fieldTargetFields, int invoiceId, string fileId, string imageFileId, string tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(fileId, nameof(fileId));
            Guard.IsNotNullOrEmpty(imageFileId, nameof(imageFileId));
            Guard.IsNotNullOrEmpty(tenantId, nameof(tenantId));
            OperationResult<GeometricFeatureCollection> featureResult;

            await tenantTrackingService.CreateIfNotExistAsync(tenantId, cancellationToken);
            var telemetry = TelemetryData.Create(invoiceId, tenantId, fileId);

            telemetryClient.TrackTrace("Recognition engine started.", telemetry);

            using (var imageStream = await documentApiClient.GetDocumentStreamAsync(imageFileId, cancellationToken))
            {
                var memoryStream = new MemoryStream();
                imageStream.CopyTo(memoryStream);
                featureResult = geometricFeaturesService.ProcessImage(memoryStream.ToArray());
            }

            if (!featureResult.IsSuccessful)
            {
                // return recognition error result
                return new OperationResult<RecognitionEngineResponse>
                {
                    Status = featureResult.Status,
                    Message = featureResult.Message
                };
            }

            var features = featureResult.Data;
            var invoiceTemplates = await invoiceTemplateRepository.GetAllAsync(tenantId, cancellationToken);
            var matchedTemplate = GetMatchedTemplate(invoiceTemplates, features);

            if (matchedTemplate != null)
            {
                telemetry.PreOcrTemplateId = matchedTemplate.Id;
                telemetry.FormRecognizerModelId = matchedTemplate.FormRecognizerModelId;
            }

            var sasResult = await documentApiClient.GetTemporaryLinkAsync(fileId, cancellationToken);
            var sasUri = sasResult.Data;
            
            // Run Invoice Document Recognition
            var formRecognizerResponse = await GetFormRecognizerResponseAsync(matchedTemplate, sasUri, telemetry, cancellationToken);
               
            if (!formRecognizerResponse.IsSucceededStatus)
            {
                telemetryClient.TrackTrace(formRecognizerResponse.ResponseDocument, telemetry);
                
                return new OperationResult<RecognitionEngineResponse>
                {
                    Message = "Form recognizer response is not successful",
                    Status = OperationResultStatus.Failed,
                    Data = null
                };
            }

            var wordList = recognitionElementFactory.CreateWords(formRecognizerResponse);
            var ocrElements = oCRElementsFactory.Create(wordList);
            ocrElements.OCRProviderRecognitionData = ocrProviderRecognitionDataFactory.Create(fieldTargetFields, formRecognizerResponse);

            // Now that we have Ocr data, we need to check if the predicted template is correct
            var keyWordCoordinates = await postOcrTemplateMatchingService.GetKeyWordCoordinatesAsync(ocrElements, cancellationToken);
            var areKeywordsValid = keyWordCoordinates.Count >= postOcrTemplateMatchingService.MinKeywordsCount;
            InvoiceTemplate postOcrTemplate = null;

            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive row lock on Tenants to be sure other transactions cannot modify or add templates for this tenant
                // InvoiceTemplates table is not really blocked for performance improvement to allow other tenants to process their invoices
                // But it should be fine since that's the only place invoice templates are created 
                await tenantTrackingService.LockAsync(tenantId, cancellationToken);

                if (areKeywordsValid)
                {
                    invoiceTemplates = await invoiceTemplateRepository.GetAllAsync(tenantId, cancellationToken);
                    postOcrTemplate = postOcrTemplateMatchingService.GetMatchingTemplate(keyWordCoordinates, invoiceTemplates);

                    if (postOcrTemplate == null && matchedTemplate != null && HasInvalidKeywords(matchedTemplate))
                    {
                        postOcrTemplate = await UpdateTemplateAsync(matchedTemplate, keyWordCoordinates, cancellationToken);
                    }
                    else if (postOcrTemplate == null)
                    {
                        postOcrTemplate = await CreateTemplateAsync(keyWordCoordinates, features, tenantId, cancellationToken);
                    }
                }
                else
                {
                    if (matchedTemplate != null && HasInvalidKeywords(matchedTemplate))
                    {
                        postOcrTemplate = matchedTemplate;
                    }
                    else
                    {
                        postOcrTemplate = await CreateTemplateAsync(keyWordCoordinates, features, tenantId, cancellationToken);
                    }
                }

                transaction.Complete();
            }

            telemetry.PostOcrTemplateId = postOcrTemplate.Id;
            telemetry.FormRecognizerModelId = postOcrTemplate.FormRecognizerModelId;

            if (postOcrTemplate.Id != matchedTemplate?.Id)
            {
                formRecognizerResponse = await GetFormRecognizerResponseAsync(postOcrTemplate, sasUri, telemetry, cancellationToken);

                if (!formRecognizerResponse.IsSucceededStatus)
                {
                    telemetryClient.TrackTrace(formRecognizerResponse.ResponseDocument, telemetry);

                    return new OperationResult<RecognitionEngineResponse>
                    {
                        Message = "Form recognizer response is not successful",
                        Status = OperationResultStatus.Failed,
                        Data = null
                    };
                }

                wordList = recognitionElementFactory.CreateWords(formRecognizerResponse);
                ocrElements = oCRElementsFactory.Create(wordList);
                ocrElements.OCRProviderRecognitionData = ocrProviderRecognitionDataFactory.Create(fieldTargetFields, formRecognizerResponse);
            }

            using (var responseStream = formRecognizerResponse.ResponseDocument.ToStream())
            {
                await trainingBlobRepository.CreateTemporaryFileAsync(fileId, responseStream, cancellationToken);
            }

            var analysisResult = invoiceAnalysisService.AnalyzeOCROutput(fieldTargetFields, ocrElements);
            var invoiceAnnotations = dataAnnotationsFactory.Create(ocrElements, analysisResult);
            var responseObject = new RecognitionEngineResponse
            {
                DataAnnotation = invoiceAnnotations,
                InvoiceTemplate = postOcrTemplate
            };

            telemetryClient.TrackTrace("Recognition engine finished.", telemetry);

            return new OperationResult<RecognitionEngineResponse>
            {
                Status = OperationResultStatus.Success,
                Data = responseObject
            };
        }

        private async Task<FormRecognizerResponse> GetFormRecognizerResponseAsync(InvoiceTemplate matchedTemplate, string sasUri, TelemetryData telemetry, CancellationToken cancellationToken)
        {
            FormRecognizerResponse response;

            using (var transaction = TransactionManager.Create())
            {
                if (matchedTemplate != null)
                {
                    // Aquire exclusive row lock on InvoiceTemplate to be sure other transactions cannot modify it (no training during recognition)
                    await invoiceTemplateRepository.LockAsync(matchedTemplate.Id, cancellationToken);

                    matchedTemplate = await invoiceTemplateRepository.GetByIdAsync(matchedTemplate.Id, cancellationToken);
                }
                
                if (string.IsNullOrEmpty(matchedTemplate?.FormRecognizerModelId) || matchedTemplate?.FormRecognizerId == null)
                {
                    //Analyze Layout method
                    response = await formRecognizerClient.RunLayoutAnalysisAsync(sasUri, cancellationToken);
                }
                else
                {
                    // Analyze Form method. We need to make sure the model is ready before sending the request to this form.
                    var modelReadinessResponse = await formRecognizerClient.AwaitModelReadinessAsync(matchedTemplate.FormRecognizerModelId, matchedTemplate.FormRecognizerId.Value, cancellationToken);
                    
                    if (modelReadinessResponse.IsModelReady)
                    {
                        response = await formRecognizerClient.RunFormAnalysisAsync(sasUri, matchedTemplate.FormRecognizerModelId, matchedTemplate.FormRecognizerId.Value, cancellationToken);
                    }
                    else
                    {
                        telemetryClient.TrackTrace(modelReadinessResponse.ResponseDocument, telemetry);

                        response = await formRecognizerClient.RunLayoutAnalysisAsync(sasUri, cancellationToken);
                    }
                }

                transaction.Complete();
            }

            return response;
        }

        private InvoiceTemplate GetMatchedTemplate(IEnumerable<InvoiceTemplate> templates, GeometricFeatureCollection features)
        {
            var closestTemplate = templates.OrderBy(template => invoiceTemplateService.GetTemplateScore(template.GeometricFeatures, features).Total).FirstOrDefault();
            
            if (closestTemplate != null)
            {
                if (!invoiceTemplateService.AreFeaturesOfSameTemplate(closestTemplate.GeometricFeatures, features))
                {
                    closestTemplate = null;
                }
            }

            return closestTemplate;
        }

        private bool HasInvalidKeywords(InvoiceTemplate template) 
        {
            return template.KeyWordCoordinates == null || template.KeyWordCoordinates.Count < postOcrTemplateMatchingService.MinKeywordsCount;
        }

        private async Task<InvoiceTemplate> UpdateTemplateAsync(InvoiceTemplate template, Dictionary<string, Coordinate> keywords, CancellationToken cancellationToken)
        {
            template.KeyWordCoordinates = keywords;
            await invoiceTemplateRepository.UpdateKeyWordCoordinatesAsync(template, cancellationToken);

            return template;
        }

        private async Task<InvoiceTemplate> CreateTemplateAsync(Dictionary<string, Coordinate> keywords, GeometricFeatureCollection features, string tenantId, CancellationToken cancellationToken)
        {
            var newTemplate = new InvoiceTemplate 
            { 
                GeometricFeatures = features, 
                TenantId = tenantId, 
                KeyWordCoordinates = keywords
            };

            await invoiceTemplateRepository.InsertAsync(newTemplate, cancellationToken);

            return newTemplate;
        }

        private readonly IFormRecognizerClient formRecognizerClient;
        private readonly IInvoiceAnalysisService invoiceAnalysisService;
        private readonly IRecognitionElementFactory recognitionElementFactory;
        private readonly IOCRElementsFactory oCRElementsFactory;
        private readonly IDataAnnotationsFactory dataAnnotationsFactory;
        private readonly IDocumentApiClient documentApiClient;
        private readonly IInvoiceTemplateService invoiceTemplateService;
        private readonly IInvoiceTemplateRepository invoiceTemplateRepository;
        private readonly IOCRProviderRecognitionDataFactory ocrProviderRecognitionDataFactory;
        private readonly ITrainingBlobRepository trainingBlobRepository;
        private readonly IPostOcrTemplateMatchingService postOcrTemplateMatchingService;
        private readonly ITelemetryClient telemetryClient;
        private readonly IGeometricFeaturesService geometricFeaturesService;
        private readonly ITenantTrackingService tenantTrackingService;
    }
}
