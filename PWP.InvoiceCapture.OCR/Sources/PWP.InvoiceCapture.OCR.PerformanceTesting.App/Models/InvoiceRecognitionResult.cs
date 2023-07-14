using System;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models
{
    internal class InvoiceRecognitionResult : RecognitionResultBase
    {
        public string FileName { get; set; }
        public string OcrTemplateId { get; set; }
        public int TotalFieldsCount { get; set; }
        public int CorrectlyAssignedFieldsCount { get; set; }
        public double CorrectlyAssignedFieldsPercent => GetPercent(CorrectlyAssignedFieldsCount, TotalFieldsCount);
        public int IncorrectlyAssignedFieldsCount { get; set; }
        public double IncorrectlyAssignedFieldsPercent => GetPercent(IncorrectlyAssignedFieldsCount, TotalFieldsCount);
        public int LineItemsCount { get; set; }
        public int FullyAssignedLineItemsCount { get; set; }
        public double FullyAssignedLineItemsPercent  => GetPercent(FullyAssignedLineItemsCount, LineItemsCount);
        public int PartiallyAssignedLineItemsCount { get; set; }
        public double PartiallyAssignedLineItemsPercent => GetPercent(PartiallyAssignedLineItemsCount, LineItemsCount);
        public TimeSpan TimeElapsed { get; set; }
    }
}
