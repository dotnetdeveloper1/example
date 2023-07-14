using PWP.InvoiceCapture.OCR.Core.FormRecognizer.Client.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Factories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Factories
{
    internal class RecognitionElementFactory : IRecognitionElementFactory
    {
        public List<WordDefinition> CreateWords(FormRecognizerResponse response)
        {
            var result = new List<WordDefinition>();
            var wordId = 0;
            var offset = 0.0f;
            var totalHeight = response.AnalyzeResult.ReadResults.Select(x => x.Height).Sum();
            var pageNumber = 1;
            var lineNumber = 0;

            foreach (var readResult in response.AnalyzeResult.ReadResults)
            {
                var documentWidth = readResult.Width;
                var documentHeight = readResult.Height;
                var unit = readResult.Unit;

                foreach (var line in readResult.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        var engineWord = CreateWord(word, offset, documentHeight, documentWidth, totalHeight, wordId, pageNumber, lineNumber, unit);
                        
                        // This checks if words are in page coordinates. 
                        // Words can exceed page boundaries if converted from other documents (a case with .xls document has been observed)
                        if(IsElementInBounds(engineWord))
                        {
                            result.Add(engineWord);
                            wordId++;
                        }
                        
                    }

                    lineNumber++;
                }

                offset += documentHeight;
                pageNumber++;
            }

            return result;
        }

        public List<TableDefinition> CreateTables(FormRecognizerResponse response)
        {
            var tables = new List<TableDefinition>();
            var tableId = 0;
            var pageNumber = 1;

            foreach (var page in response.AnalyzeResult.PageResults)
            {
                if (page.Tables == null)
                {
                    continue;
                }

                foreach (var table in page.Tables)
                {
                    var tableDefinition = new TableDefinition
                    {
                        Id = tableId,
                        PageNumber = pageNumber,
                        ColumnsCount = table.Columns,
                        RowsCount = table.Rows,
                        Cells = CreateTableCells(page, table, response)
                    };

                    tableId++;
                    tables.Add(tableDefinition);
                }

                pageNumber++;
            }

            return tables;
        }

        private List<TableDefinitionCell> CreateTableCells(PageResult page, Table table, FormRecognizerResponse response) 
        {
            var pageResult = response.AnalyzeResult.ReadResults
                .Where(readResult => readResult.Page == page.Page)
                .FirstOrDefault();

            // This shouldn't be happened but just in case FormRecognizer returns null or incorrect values not to break the overall recognition
            if (pageResult == null || table.Cells == null)
            {
                return new List<TableDefinitionCell>();
            }

            return table.Cells
                .Select((cell, index) => CreateTableCell(pageResult, cell, index))
                .Where(tableCell => IsElementInBounds(tableCell))
                .ToList();
        }

        private TableDefinitionCell CreateTableCell(ReadResult page, Cell cell, int cellId) 
        {
            CheckCoordinateValidity(cell.BoundingBox);

            var tableCell = new TableDefinitionCell
            {
                Id = cellId,
                ColumnIndex = cell.ColumnIndex,
                RowIndex = cell.RowIndex,
                ColumnSpan = cell.ColumnSpan,
                RowSpan = cell.RowSpan,
                Text = cell.Text,
                WordIds = cell.Elements
            };

            SetPageLevelNormalizedCoordinates(tableCell, cell.BoundingBox, page.Width, page.Height);

            return tableCell;
        }

        private WordDefinition CreateWord(Word formRecognizerWord, float offset, float documentHeight, float documentWidth, float totalHeight, int wordId, int pageNo, int lineNo, string unit)
        {
            var engineWord = new WordDefinition();

            // Some pdf files have their Right and Left coordinates inverted. We check if Right coordinate is lower than Left and swap them if that's the case
            // Same thing is done for Top and Bottom
            CheckCoordinateValidity(formRecognizerWord.BoundingBox);
            SetPageLevelNormalizedCoordinates(engineWord, formRecognizerWord.BoundingBox, documentWidth, documentHeight);

            engineWord.DocumentLevelNormalizedLeft = formRecognizerWord.BoundingBox[0] / documentWidth;
            engineWord.DocumentLevelNormalizedRight = formRecognizerWord.BoundingBox[4] / documentWidth;
            engineWord.DocumentLevelNormalizedTop = (offset + formRecognizerWord.BoundingBox[1]) / totalHeight;
            engineWord.DocumentLevelNormalizedBottom = (offset + formRecognizerWord.BoundingBox[5]) / totalHeight;

            engineWord.PageLevelPixelsLeft = GetCoordinateInPixels(formRecognizerWord.BoundingBox[0], unit);
            engineWord.PageLevelPixelsRight = GetCoordinateInPixels(formRecognizerWord.BoundingBox[4], unit);
            engineWord.PageLevelPixelsTop = GetCoordinateInPixels(formRecognizerWord.BoundingBox[1], unit);
            engineWord.PageLevelPixelsBottom = GetCoordinateInPixels(formRecognizerWord.BoundingBox[5], unit);

            engineWord.PageNumber = pageNo;          
            engineWord.Id = wordId;
            formRecognizerWord.InternalId = wordId;
            engineWord.Text = formRecognizerWord.Text;
            engineWord.DataType = formRecognizerWord.Text.GetDataType();
            engineWord.LineNo = lineNo;
            
            return engineWord;
        }

        private void SetPageLevelNormalizedCoordinates(RecognitionElementBase element, List<float> boundingBox, float documentWidth, float documentHeight) 
        {
            element.PageLevelNormalizedLeft = boundingBox[0] / documentWidth;
            element.PageLevelNormalizedTop = boundingBox[1] / documentHeight;
            element.PageLevelNormalizedRight = boundingBox[4] / documentWidth;
            element.PageLevelNormalizedBottom = boundingBox[5] / documentHeight;
        }

        private bool IsElementInBounds(RecognitionElementBase element) => 
            element.PageLevelNormalizedRight <= 1.0f && element.PageLevelNormalizedLeft >= 0.0f && 
            element.PageLevelNormalizedTop >= 0.0f && element.PageLevelNormalizedBottom <= 1.0f;

        private void CheckCoordinateValidity(List<float> boundingBox)
        {  
            if(boundingBox[rightCoordinateIndex] < boundingBox[leftCoordinateIndex])
            {
                SwapCoordinates(boundingBox, rightCoordinateIndex, leftCoordinateIndex);
            }

            if (boundingBox[bottomCoordinateIndex] < boundingBox[topCoordinateIndex])
            {
                SwapCoordinates(boundingBox, bottomCoordinateIndex, topCoordinateIndex);
            }
        }

        private void SwapCoordinates(List<float> boundingBox, int first, int second)
        {
            var temp = boundingBox[first];
            boundingBox[first] = boundingBox[second];
            boundingBox[second] = temp;
        }

        // The unit used by the width, height and boundingBox properties. For images, the unit is "pixel". For PDF, the unit is "inch".
        private float GetCoordinateInPixels(float value, string unit) 
        {
            switch (unit.ToLower())
            {
                case "inch":
                    return value * pixelsPerInch;
                case "pixel":
                    return value;
            }

            throw new ArgumentException($"Cannot convert to pixels. Unknown unit = {unit}");
        }

        private readonly int leftCoordinateIndex = 0;
        private readonly int rightCoordinateIndex = 4;
        private readonly int topCoordinateIndex = 1;
        private readonly int bottomCoordinateIndex = 5;
        private const float pixelsPerInch = 150f;
    }
}
