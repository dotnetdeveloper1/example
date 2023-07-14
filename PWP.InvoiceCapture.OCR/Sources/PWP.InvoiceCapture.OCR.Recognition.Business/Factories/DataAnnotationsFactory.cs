using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Factories
{
    public class DataAnnotationsFactory : IDataAnnotationsFactory
    {
        public DataAnnotationsFactory()
        {  
        }
        
        public DataAnnotation Create(OCRElements ocrElements, AnalysisResult analysisResult)
        {
            Guard.IsNotNull(ocrElements, nameof(ocrElements));
            Guard.IsNotNull(analysisResult, nameof(analysisResult));

            var annotations = new DataAnnotation
            {
                PlainDocumentText = ocrElements.RawText,
                DocumentLayoutItems = new List<DocumentLayoutItem>(),
                InvoiceAnnotations = new List<Annotation>(),
                Tables = CreateTables(ocrElements.OCRProviderRecognitionData?.Tables)
            };

            CreateLayoutItems(ocrElements.Words, annotations);
            CreateInvoiceAnnotations(analysisResult, annotations);
            CreateLineItemAnnotations(analysisResult, annotations);

            return annotations;
        }

        private List<Table> CreateTables(List<TableDefinition> tables) 
        {
            if (tables == null)
            {
                return null;
            }

            return tables
                .Select(CreateTable)
                .ToList();
        }

        private Table CreateTable(TableDefinition table) 
        {
            return new Table
            {
                Id = table.Id.ToString(),
                ColumnsCount = table.ColumnsCount,
                RowsCount = table.RowsCount,
                PageNumber = table.PageNumber,
                Cells = table.Cells.Select(CreateTableCell).ToList()
            };
        }

        private TableCell CreateTableCell(TableDefinitionCell cell) 
        {
            return new TableCell
            {
                Id = cell.Id,
                ColumnIndex = cell.ColumnIndex,
                RowIndex = cell.RowIndex,
                ColumnSpan = cell.ColumnSpan,
                RowSpan = cell.RowSpan,
                Text = cell.Text,
                DocumentLayoutItemIds = cell.WordIds,
                TopLeft = new DocumentLayoutPoint { X = cell.PageLevelNormalizedLeft, Y = cell.PageLevelNormalizedTop },
                BottomRight = new DocumentLayoutPoint { X = cell.PageLevelNormalizedRight, Y = cell.PageLevelNormalizedBottom }
            };
        }

        private Annotation GetLineItemAnnotation(LineItem item, string fieldName)
        {
            return new Annotation
            {
                DocumentLayoutItemIds = item.WordIds.Select(wordId => wordId.ToString()).ToList(),
                FieldType = fieldName,
                FieldValue = item.Value.GetAnnotatedValue(false),
                UserCreated = false
            };
        }

        private void CreateLineItemAnnotations(AnalysisResult analysisResult, DataAnnotation annotations)
        {
            annotations.InvoiceLineAnnotations = new List<LineAnnotation>();
            
            if (analysisResult.LineItemsRows != null)
            {   
                var orderNumber = 1;
                var lineItemsRows = analysisResult.LineItemsRows;

                foreach (var row in lineItemsRows)
                {
                    var lineAnnotation = CreateLineAnnotation(row, orderNumber);

                    annotations.InvoiceLineAnnotations.Add(lineAnnotation);
                    orderNumber++;
                }
            }
        }

        private LineAnnotation CreateLineAnnotation(LineItemRow row, int orderNumber) 
        {
            var lineAnnotation = new LineAnnotation
            {
                OrderNumber = orderNumber,
                LineItemAnnotations = new List<Annotation>()
            };

            if (row.Description != null)
            {
                lineAnnotation.LineItemAnnotations.Add(
                    GetLineItemAnnotation(row.Description, InvoiceLineFieldTypes.Description));
            }

            if (row.ItemNumber != null)
            {
                lineAnnotation.LineItemAnnotations.Add(
                    GetLineItemAnnotation(row.ItemNumber, InvoiceLineFieldTypes.Number));
            }

            if (row.Quantity != null)
            {
                lineAnnotation.LineItemAnnotations.Add(
                    GetLineItemAnnotation(row.Quantity, InvoiceLineFieldTypes.Quantity));
            }

            if (row.TotalPrice != null)
            {
                lineAnnotation.LineItemAnnotations.Add(
                    GetLineItemAnnotation(row.TotalPrice, InvoiceLineFieldTypes.Total));
            }

            if (row.UnitPrice != null)
            {
                lineAnnotation.LineItemAnnotations.Add(
                    GetLineItemAnnotation(row.UnitPrice, InvoiceLineFieldTypes.Price));
            }

            return lineAnnotation;
        }

        private void CreateLayoutItems(IEnumerable<WordDefinition> words, DataAnnotation annotations)
        {
            foreach (var word in words)
            {
                var layoutItem = new DocumentLayoutItem
                {
                    Id = word.Id.ToString(),
                    Text = word.Text,
                    Value = word.Text.GetAnnotatedValue(false),
                    PageNumber = word.PageNumber,
                    TopLeft = new DocumentLayoutPoint { X = word.PageLevelNormalizedLeft, Y = word.PageLevelNormalizedTop },
                    BottomRight = new DocumentLayoutPoint { X = word.PageLevelNormalizedRight, Y = word.PageLevelNormalizedBottom }
                };
                
                annotations.DocumentLayoutItems.Add(layoutItem);
            }
        }

        private void CreateInvoiceAnnotations(AnalysisResult analysisResult, DataAnnotation annotations)
        {
            var labelsKeyValues = GetFoundLabelsOrderedByValueProbabilityDescending(analysisResult.Labels);

            foreach (var keyValue in labelsKeyValues)
            {
                var matchedWordGroup = keyValue.Value.First();

                var matchedWordGroupWordIds = matchedWordGroup.Words
                    .Select(word => word.Id.ToString())
                    .ToList();

                var isAlreadyAssigned = annotations.InvoiceAnnotations
                    .SelectMany(annotation => annotation.DocumentLayoutItemIds)
                    .Intersect(matchedWordGroupWordIds)
                    .Any();

                if (isAlreadyAssigned)
                {
                    continue;
                }

                var invoiceAnnotation = CreateAnnotation(keyValue.Key, matchedWordGroup, matchedWordGroupWordIds);

                annotations.InvoiceAnnotations.Add(invoiceAnnotation);
            }
        }

        private List<KeyValuePair<FieldTargetField, List<WordGroup>>> GetFoundLabelsOrderedByValueProbabilityDescending(Dictionary<FieldTargetField, List<WordGroup>> labels) 
        {
            var labelsWithNonEmptyWordsInWordGroups = labels
                .Select(label =>
                    new KeyValuePair<FieldTargetField, List<WordGroup>>(
                        label.Key,
                        label.Value.Where(wordGroup => wordGroup.Words.Count > 0).ToList()));

            return labelsWithNonEmptyWordsInWordGroups
                .Where(label => label.Value.Count > 0)
                .Select(label =>
                    new KeyValuePair<FieldTargetField, List<WordGroup>>(
                        label.Key,
                        label.Value.OrderByDescending(value => value.ValueProbability).ToList()))
                .OrderByDescending(label => label.Value.First().ValueProbability)
                .ToList();
        }

        private Annotation CreateAnnotation(FieldTargetField fieldTargetField, WordGroup matchedWordGroup, List<string> documentLayoutItemIds)
        {
            var fieldValue = fieldTargetField.DataType == DataType.Number
                    ? matchedWordGroup.Text.GetAnnotatedValue(true)
                    : matchedWordGroup.Text;

            return new Annotation
            {
                FieldType = fieldTargetField.FieldId.ToString(),
                FieldValue = fieldValue,
                FieldName = fieldTargetField.FieldName,
                DocumentLayoutItemIds = documentLayoutItemIds,
                UserCreated = false
            };
        }
    }
}
