using System.Collections.Generic;
using System.Linq;
using InvoiceProcessingResultV1_0 = PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models.InvoiceProcessingResult;
using InvoiceProcessingResultV0_9 = PWP.InvoiceCapture.InvoiceManagement.API.Versions.V0_9.Models.InvoiceProcessingResult;

namespace PWP.InvoiceCapture.InvoiceManagement.API.Versions.V0_9.Mappers
{
    public class ProcessingResultMapper
    {
        public static List<InvoiceProcessingResultV0_9> ToV0_9Models(List<InvoiceProcessingResultV1_0> processingResult)
        {
            return processingResult.Select(processingResult => ToV0_9Model(processingResult)).ToList();
        }

        public static InvoiceProcessingResultV0_9 ToV0_9Model(InvoiceProcessingResultV1_0 processingResult)
        {
            var result = new InvoiceProcessingResultV0_9()
            {
                Id = processingResult.Id,
                InvoiceId = processingResult.InvoiceId,
                TemplateId = processingResult.TemplateId,
                VendorName = processingResult.VendorName,
                CultureName = processingResult.CultureName,
                ProcessingType = processingResult.ProcessingType,
                TrainingFileCount = processingResult.TrainingFileCount,
                Invoice = InvoiceMapper.ToV0_9Model(processingResult.Invoice),
                DataAnnotation = processingResult.DataAnnotation,
                DataAnnotationFileId = processingResult.DataAnnotationFileId,
                InitialDataAnnotationFileId = processingResult.InitialDataAnnotationFileId,
                CreatedDate = processingResult.CreatedDate,
                ModifiedDate = processingResult.ModifiedDate
            };

            return result;
        }
    }
}
