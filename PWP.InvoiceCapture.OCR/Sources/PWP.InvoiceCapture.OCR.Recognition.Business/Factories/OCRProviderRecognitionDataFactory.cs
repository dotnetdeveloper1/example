using Azure.AI.FormRecognizer.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Core.Models.FormRecognizer;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Enumerations;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Factories
{
    internal class FormRecognizerRecognitionDataFactory : IOCRProviderRecognitionDataFactory
    {
        public FormRecognizerRecognitionDataFactory(IFormRecognizerKeyConversionService keyConversionService, IRecognitionElementFactory recognitionElementFactory)
        {
            Guard.IsNotNull(keyConversionService, nameof(keyConversionService));
            Guard.IsNotNull(recognitionElementFactory, nameof(recognitionElementFactory));

            this.keyConversionService = keyConversionService;
            this.recognitionElementFactory = recognitionElementFactory;
        }

        public OCRProviderRecognitionData Create(List<FieldTargetField> fieldTargetFields, OCRProviderResponse ocrProviderResponse)
        {
            Guard.IsNotNull(ocrProviderResponse, nameof(ocrProviderResponse));

            var formRecognizerResponse = (FormRecognizerResponse)ocrProviderResponse;
            var analyzeResult = formRecognizerResponse.AnalyzeResult;
            var recognitionData = new OCRProviderRecognitionData();

            if (analyzeResult.DocumentResults != null && analyzeResult.DocumentResults.Count > 0)
            {
                recognitionData.LineItemsRows = GetLineItemsRows(analyzeResult);
                recognitionData.FormValues = GetFormValuesFromAnalyzeResult(fieldTargetFields, analyzeResult);
            }
            else
            {
                recognitionData.FormValues = GetFormValuesFromInvoiceFields(fieldTargetFields, formRecognizerResponse.InvoiceFields, formRecognizerResponse.AnalyzeResult);
            }

            if (isTableRecognitionEnabled && analyzeResult.PageResults != null && analyzeResult.PageResults.Count > 0)
            {
                recognitionData.Tables = GetTables(formRecognizerResponse);
            }

            return recognitionData;
        }

        private LineItem GetLineItemFromDocumentResultItem(DocumentResultItem docResultItem, AnalyzeResult analyzeResult)
        {
            if (docResultItem == null)
            {
                return null;
            }

            var pageHeight = analyzeResult.ReadResults[docResultItem.Page-1].Height;
            var pageWidth = analyzeResult.ReadResults[docResultItem.Page-1].Width;

            return new LineItem
            {
                Value = docResultItem.Text,
                PageNo = docResultItem.Page,
                Left = docResultItem.BoundingBox[0]/ pageWidth,
                Right = docResultItem.BoundingBox[2]/ pageWidth,
                Top = docResultItem.BoundingBox[1]/ pageHeight,
                Bottom = docResultItem.BoundingBox[7]/ pageHeight,
                WordIds = GetWordIdsFromElementReferences(docResultItem.Elements, analyzeResult)
            };
        }

        private FormValue GetFormValueFromDocumentResultItem(DocumentResultItem docResultItem,string labelName, AnalyzeResult analyzeResult)
        {
            if (docResultItem == null)
            {
                return new FormValue
                {
                    LabelName = labelName,
                    WordIds = null,
                    LabelValue = null
                };
            }

            var pageHeight = analyzeResult.ReadResults[docResultItem.Page-1].Height;
            var pageWidth = analyzeResult.ReadResults[docResultItem.Page - 1].Width;

            return new FormValue
            {
                PageNo = docResultItem.Page,
                Left = docResultItem.BoundingBox[0] / pageWidth,
                Right = docResultItem.BoundingBox[2] / pageWidth,
                Top = docResultItem.BoundingBox[1] / pageHeight,
                Bottom = docResultItem.BoundingBox[7] / pageHeight,
                LabelName = labelName,
                LabelValue = docResultItem.Text,
                WordIds = GetWordIdsFromElementReferences(docResultItem.Elements, analyzeResult)
            };
        }

        private List<int> GetWordIdsFromElementReferences(List<string> references, AnalyzeResult analyzeResult)
        {
            // the element template is the following
            // @"#/analyzeResult/readResults/0/lines/156/words/0";
            // we just need to extract the number fields and they will correspond to readresults, lines and words respectively
            var wordIds = new List<int>();

            foreach(var reference in references)
            {
                var matches = Regex.Matches(reference, "[0-9]+");
                var readIndex = int.Parse(matches[0].Value);
                var lineIndex = int.Parse(matches[1].Value);
                var wordIndex = int.Parse(matches[2].Value);
                var wordId = analyzeResult.ReadResults[readIndex].Lines[lineIndex].Words[wordIndex].InternalId;
                wordIds.Add(wordId);
            }
            
            return wordIds;
        }

        private List<LineItemRow> GetLineItemsRows(AnalyzeResult response)
        {
            var documentResults = response.DocumentResults;
            var lineItemsRows = new List<LineItemRow>();

            foreach (var docResult in documentResults)
            {
                var lineCount = keyConversionService.GetNumberOfLineItems(docResult.Fields);

                for (var lineIndex = 0; lineIndex < lineCount; lineIndex++)
                {
                    var itemNo = keyConversionService.GetItemNo(lineIndex, docResult.Fields);
                    var description = keyConversionService.GetDescription(lineIndex, docResult.Fields);
                    var unitPrice = keyConversionService.GetUnitPrice(lineIndex, docResult.Fields);
                    var quantity = keyConversionService.GetQuantity(lineIndex, docResult.Fields);
                    var totalPrice = keyConversionService.GetTotalPrice(lineIndex, docResult.Fields);
                    
                    var row = new LineItemRow
                    {
                        ItemNumber = GetLineItemFromDocumentResultItem(itemNo, response),
                        Description = GetLineItemFromDocumentResultItem(description, response),
                        Quantity = GetLineItemFromDocumentResultItem(quantity, response),
                        TotalPrice = GetLineItemFromDocumentResultItem(totalPrice, response),
                        UnitPrice = GetLineItemFromDocumentResultItem(unitPrice, response)
                    };

                    lineItemsRows.Add(row);
                }
            }

            return lineItemsRows;
        }

        private List<FormValue> GetFormValuesFromAnalyzeResult(List<FieldTargetField> fieldTargetFields, AnalyzeResult response)
        {
            var documentResults = response.DocumentResults;
            var formValues = new List<FormValue>();

            foreach (var docResult in documentResults)
            {
                var docResultFormValues = keyConversionService.GetFormItems(fieldTargetFields, docResult.Fields);
                foreach (var docResultFormValue in docResultFormValues)
                {
                    var formValue = GetFormValueFromDocumentResultItem(docResultFormValue.Value, docResultFormValue.Key, response);
                    formValues.Add(formValue);
                }
            }

            return formValues;
        }

        private List<FormValue> GetFormValuesFromInvoiceFields(List<FieldTargetField> fieldTargetFields, Dictionary<string, FormField> invoiceFields, AnalyzeResult analyzeResult) 
        {
            var targetFieldNameFieldIds = fieldTargetFields
                .Where(fieldTargetField => fieldTargetField.FormRecognizerFieldType != FormRecognizerFieldType.Unknown)
                .ToDictionary(fieldTargetField => fieldTargetField.TargetFieldName, fieldTargetField => fieldTargetField.FieldId);

            return invoiceFields
                .Where(invoiceField => targetFieldNameFieldIds.ContainsKey(invoiceField.Key))
                .Select(invoiceField => GetFormValueFromFormField(targetFieldNameFieldIds, invoiceField.Key, invoiceField.Value, analyzeResult))
                .ToList();
        }

        private FormValue GetFormValueFromFormField(Dictionary<string, int> targetFieldNameFieldIds, string key, FormField formField, AnalyzeResult analyzeResult)
        {
            var valueData = formField.ValueData;
            var pageHeight = analyzeResult.ReadResults[valueData.PageNumber - 1].Height;
            var pageWidth = analyzeResult.ReadResults[valueData.PageNumber - 1].Width;

            return new FormValue
            {
                PageNo = valueData.PageNumber,
                Left = valueData.BoundingBox[0].X / pageWidth,
                Top = valueData.BoundingBox[0].Y / pageHeight,
                Right = valueData.BoundingBox[2].X / pageWidth,
                Bottom = valueData.BoundingBox[2].Y / pageHeight,
                LabelName = targetFieldNameFieldIds[key].ToString(),
                LabelValue = valueData.Text,
                WordIds = GetWordIdsByBoundingBoxes(valueData, analyzeResult)
            };
        }

        private List<int> GetWordIdsByBoundingBoxes(FieldData valueData, AnalyzeResult analyzeResult)
        {
            var left = MinRound(valueData.BoundingBox[0].X, valueData.BoundingBox[3].X);
            var top = MinRound(valueData.BoundingBox[0].Y, valueData.BoundingBox[1].Y);
            var right = MaxRound(valueData.BoundingBox[2].X, valueData.BoundingBox[1].X);
            var bottom = MaxRound(valueData.BoundingBox[2].Y, valueData.BoundingBox[3].Y);

            return analyzeResult.ReadResults[valueData.PageNumber - 1].Lines
                .SelectMany(line => line.Words)
                .Where(word =>
                    MaxRound(word.BoundingBox[0], word.BoundingBox[6]) >= left &&
                    MaxRound(word.BoundingBox[1], word.BoundingBox[3]) >= top &&
                    MinRound(word.BoundingBox[2], word.BoundingBox[4]) <= right &&
                    MinRound(word.BoundingBox[7], word.BoundingBox[5]) <= bottom &&
                    valueData.Text.Contains(word.Text))
                .Select(word => word.InternalId)
                .ToList();
        }

        private List<TableDefinition> GetTables(FormRecognizerResponse response)
        {
            var tables = recognitionElementFactory.CreateTables(response);

            foreach (var table in tables)
            {
                foreach (var cell in table.Cells)
                {
                    var wordIds = GetWordIdsFromElementReferences(cell.WordIds, response.AnalyzeResult);

                    cell.WordIds = wordIds
                        .Select(wordId => wordId.ToString())
                        .ToList();
                }
            }

            return tables;
        }


        private double MinRound(double value1, double value2) => Round(Math.Min(value1, value2));

        private double MaxRound(double value1, double value2) => Round(Math.Max(value1, value2));

        private double Round(double value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        // TODO: Temporary disabled Tables in OCR services. Should be reenabled if required or fully removed once this feature is done on Frontend
        private const bool isTableRecognitionEnabled = false;

        private readonly IFormRecognizerKeyConversionService keyConversionService;
        private readonly IRecognitionElementFactory recognitionElementFactory;
    }
}
