using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Enumerations;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Services
{
    internal class InvoiceAnalysisService : IInvoiceAnalysisService
    {
        public AnalysisResult AnalyzeOCROutput(List<FieldTargetField> fieldTargetFields, OCRElements input)
        {
            var result = fieldTargetFields.ToDictionary(fieldTargetField => fieldTargetField, fieldTargetField => new List<WordGroup>());

            // First add the form values supplied from the ocr provider
            // We do not call other algorithm in this case because they don't support trainng so not to affect user's choice 
            if (input.OCRProviderRecognitionData?.FormValues != null)
            {
                AddResultFromOCRProviderRecognitionData(input, result);
            }

            // TODO: We comment out this code because sometimes it return worst results. It takes too much time to support this code.
            //else
            //{
            //    // Apply other algorithms on the labels in case no OCR Provider output
            //    var ngramExtractionResult = await extractorService.ExtractLabelsFromNGramsAsync(input, cancellationToken);
            //    AddResult(result, ngramExtractionResult);

            //    var labelExtractionResult = await extractorService.ExtractLabelsAsync(input, cancellationToken);
            //    AddResult(result, labelExtractionResult);

            //    var pairExtractionResult = await extractorService.ExtractLabelsFromPairsAsync(input, cancellationToken);
            //    AddResult(result, pairExtractionResult);
            //}

            return new AnalysisResult 
            { 
                Labels = result, 
                LineItemsRows = input.OCRProviderRecognitionData?.LineItemsRows 
            };
        }

        private void AddResultFromOCRProviderRecognitionData(OCRElements ocrInput, Dictionary<FieldTargetField, List<WordGroup>> initialResult)
        {
            var ocrProviderRecognitionData = ocrInput.OCRProviderRecognitionData;

            var formValuesWithWords = ocrProviderRecognitionData.FormValues.Where(formValue => formValue.WordIds != null).ToList();

            foreach (var formValue in formValuesWithWords)
            {
                var fieldTargetField = initialResult.Keys.FirstOrDefault(field => string.Equals(field.FieldId.ToString(), formValue.LabelName));
                if(fieldTargetField == null)
                {
                    // this shouldn't happen once the training is done from the system instead of manually.
                    // it means there is a mismatch between the keys we store in formrecognizer training and the labels we are searching for
                    continue;
                }
                var wordsReturned = new WordGroup(ocrInput.Words.Where(word => formValue.WordIds.Contains(word.Id)).ToList());
                initialResult[fieldTargetField].Add(wordsReturned);
            }
        }

        // TODO: see previous TODO comment
        //private void AddResult(Dictionary<FieldTargetField, List<WordGroup>> previousResult, Dictionary<FieldTargetField, List<WordGroup>> newResult)
        //{
        //    foreach (var key in newResult.Keys)
        //    {
        //        if (newResult[key].Count > 0)
        //        {
        //            var oldResult = previousResult.FirstOrDefault(result => result.Key.FieldId == key.FieldId);

        //            if (oldResult.Value.Count > 0)
        //            {
        //                continue;
        //            }

        //            oldResult.Value.AddRange(newResult[key]);
        //        }
        //    }
        //}
    }
}
