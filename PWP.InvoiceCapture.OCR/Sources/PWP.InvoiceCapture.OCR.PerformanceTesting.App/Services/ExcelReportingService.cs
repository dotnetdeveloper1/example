using ClosedXML.Excel;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Definitions;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Services
{
    internal class ExcelReportingService : IReportingService
    {
        public string Extension => ".xlsx";

        public byte[] Create(RecognitionTestResult results)
        {
            using (var excelWorkbook = new XLWorkbook(fileName))
            {
                AddInvoiceGroups(excelWorkbook, results);
                AddGlobalStatistic(excelWorkbook, results);
                
                using (var memoryStream = new MemoryStream())
                {
                    excelWorkbook.SaveAs(memoryStream);

                    return memoryStream.ToArray();
                }
            }
        }

        private static IXLWorksheet GetExcelSheet(XLWorkbook excelWorkbook, int sheetId)
        {
            var workSheet = excelWorkbook.Worksheet(sheetId);

            if (workSheet == null)
            {
                throw new Exception($"There is no worksheet {sheetId} in excel template file.");
            }

            return workSheet;
        }

        private void AddInvoiceGroups(XLWorkbook excelWorkbook, RecognitionTestResult result)
        {
            var workSheet = GetExcelSheet(excelWorkbook, invoiceGroupsSheetNumber);
            var rowNumber = workSheet.LastRowUsed().RowNumber();

            foreach (var invoiceGroup in result.InvoiceGroups)
            {
                var color = GetRowsColor(invoiceGroup);

                rowNumber += AddInvoiceGroupRows(rowNumber, invoiceGroup.Folder, invoiceGroup.Results, workSheet, color);
            }
        }

        private void AddGlobalStatistic(XLWorkbook excelWorkbook, RecognitionTestResult result)
        {
            var workSheet = GetExcelSheet(excelWorkbook, statisticsSheetNumber);

            SetCell(2, 2, result.GroupWithAccuratelyRecognizedInvoicesCount, workSheet);
            SetCell(2, 3, result.TotalGroupCount, workSheet);
            SetCell(2, 4, result.GroupsWithAccurateInvoiceRecognitionPercent, workSheet, null, percentageNumberFormatId);
            
            SetCell(3, 2, result.SameTemplateGroupCount, workSheet);
            SetCell(3, 3, result.TotalGroupCount, workSheet);
            SetCell(3, 4, result.GroupsOfSameTemplatePercent, workSheet, null, percentageNumberFormatId);

            SetCell(5, 2, result.MaxRecognitionSpeed, workSheet);
            SetCell(6, 2, result.MinRecognitionSpeed, workSheet);
            SetCell(7, 2, result.AverageRecognitionSpeed, workSheet);
        }

        private int AddInvoiceGroupRows(int rowNumber, string folder, List<InvoiceRecognitionResult> invoices, IXLWorksheet workSheet, XLColor color)
        {
            if (invoices == null || invoices.Count < 1)
            {
                return invoices.Count;
            }

            var uploadAttempt = 1;

            foreach (var invoice in invoices)
            {
                var rowIndex = rowNumber + 1;

                AddRow(workSheet, rowIndex, rowNumber, invoice, folder, uploadAttempt, color);
                rowNumber++;
                uploadAttempt++;
            }

            return invoices.Count;
        }

        private void AddRow(IXLWorksheet workSheet, int rowIndex, int rowNumber, InvoiceRecognitionResult invoice, string groupName, int uploadAttempt, XLColor rowColor) 
        {
            SetCell(rowIndex, 1, rowNumber, workSheet, rowColor);
            SetCell(rowIndex, 2, uploadAttempt, workSheet, rowColor);
            SetCell(rowIndex, 3, invoice.FileName, workSheet, rowColor);
            SetCell(rowIndex, 4, groupName, workSheet, rowColor);
            SetCell(rowIndex, 5, invoice.OcrTemplateId, workSheet, rowColor);
            SetCell(rowIndex, 6, invoice.TotalFieldsCount, workSheet, rowColor);
            SetCell(rowIndex, 7, invoice.CorrectlyAssignedFieldsCount, workSheet, rowColor);

            var cellColor = GetCellColor(invoice, uploadAttempt);
            SetCell(rowIndex, 8, invoice.CorrectlyAssignedFieldsPercent, workSheet, cellColor ?? rowColor, percentageNumberFormatId);
            
            SetCell(rowIndex, 9, invoice.IncorrectlyAssignedFieldsCount, workSheet, rowColor);
            SetCell(rowIndex, 10, invoice.IncorrectlyAssignedFieldsPercent, workSheet, rowColor, percentageNumberFormatId);
            SetCell(rowIndex, 11, invoice.LineItemsCount, workSheet, rowColor);
            SetCell(rowIndex, 12, invoice.FullyAssignedLineItemsCount, workSheet, rowColor);
            SetCell(rowIndex, 13, invoice.FullyAssignedLineItemsPercent, workSheet, rowColor, percentageNumberFormatId);
            SetCell(rowIndex, 14, invoice.PartiallyAssignedLineItemsCount, workSheet, rowColor);
            SetCell(rowIndex, 15, invoice.PartiallyAssignedLineItemsPercent, workSheet, rowColor, percentageNumberFormatId);
            SetCell(rowIndex, 16, invoice.TimeElapsed, workSheet, rowColor);
        }

        private void SetCell<TValue>(int rowIndex, int columnIndex, TValue value, IXLWorksheet workSheet, XLColor color = null, int? numberFormatId = null)
        {
            var cell = workSheet.Cell(rowIndex, columnIndex);

            cell.SetValue(value);

            if (color != null)
            {
                cell.Style.Fill.SetBackgroundColor(color);
            }

            if (numberFormatId.HasValue)
            {
                cell.Style.NumberFormat.NumberFormatId = numberFormatId.Value;
            }

            SetBorders(workSheet, rowIndex, columnIndex);
        }

        private void SetBorders(IXLWorksheet workSheet, int rowIndex, int columnIndex) 
        {
            workSheet.Cell(rowIndex, columnIndex).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            workSheet.Cell(rowIndex, columnIndex).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            workSheet.Cell(rowIndex, columnIndex).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            workSheet.Cell(rowIndex, columnIndex).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        }

        private XLColor GetRowsColor(InvoiceGroupRecognitionResult group)
        {
            return group.IsSameTemplate
                ? XLColor.LightGreen
                : XLColor.Orange;
        }

        private XLColor GetCellColor(InvoiceRecognitionResult result, int uploadAttempt)
        {
            return uploadAttempt >= CommonDefinitions.UploadAttemptToCheckRecognition && result.CorrectlyAssignedFieldsPercent <= CommonDefinitions.RecognitionPercentThreshold
                ? XLColor.Red
                : null;
        }

        private const string fileName = @"Data\OCR Performance Results Empty Template.xlsx";
        private const int invoiceGroupsSheetNumber = 1;
        private const int statisticsSheetNumber = 2;
        private const int percentageNumberFormatId = 10;
    }
}
