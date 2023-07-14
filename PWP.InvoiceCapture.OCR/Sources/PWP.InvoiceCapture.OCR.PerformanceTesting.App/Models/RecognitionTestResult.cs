using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models
{
    internal class RecognitionTestResult : RecognitionResultBase
    {
        public List<InvoiceGroupRecognitionResult> InvoiceGroups { get; set; }
        public int TotalGroupCount => InvoiceGroups.Count;
        public int SameTemplateGroupCount => InvoiceGroups.Count(x => x.IsSameTemplate);
        public double GroupsOfSameTemplatePercent => GetPercent(SameTemplateGroupCount, TotalGroupCount);
        public int GroupWithAccuratelyRecognizedInvoicesCount => GetAccuratelyRecognizedCount();
        public double GroupsWithAccurateInvoiceRecognitionPercent => GetPercent(GroupWithAccuratelyRecognizedInvoicesCount, TotalGroupCount);
        public TimeSpan MaxRecognitionSpeed => GetMaxRecognitionSpeed();
        public TimeSpan MinRecognitionSpeed => GetMinRecognitionSpeed();
        public TimeSpan AverageRecognitionSpeed => GetAverageRecognitionSpeed();

        private int GetAccuratelyRecognizedCount()
        {
            if (InvoiceGroups == null)
            {
                return 0;
            }

            return InvoiceGroups
                .Where(group => group.Results
                    .Where((item, index) => 
                        item.CorrectlyAssignedFieldsPercent < CommonDefinitions.RecognitionPercentThreshold && 
                        index + 1 >= CommonDefinitions.UploadAttemptToCheckRecognition)
                    .Count() == 0)
                .Count();
        }

        private TimeSpan GetMaxRecognitionSpeed() 
        {
            if (InvoiceGroups == null || InvoiceGroups.Count == 0)
            {
                return TimeSpan.Zero;
            }

            return InvoiceGroups
                .SelectMany(group => group.Results)
                .Min(invoice => invoice.TimeElapsed);
        }

        private TimeSpan GetMinRecognitionSpeed()
        {
            if (InvoiceGroups == null || InvoiceGroups.Count == 0)
            {
                return TimeSpan.Zero;
            }

            return InvoiceGroups
                .SelectMany(group => group.Results)
                .Max(invoice => invoice.TimeElapsed);
        }

        private TimeSpan GetAverageRecognitionSpeed()
        {
            if (InvoiceGroups == null || InvoiceGroups.Count == 0)
            {
                return TimeSpan.Zero;
            }

            var invoices = InvoiceGroups
                .SelectMany(group => group.Results)
                .ToList();

            if (invoices.Count == 0)
            {
                return TimeSpan.Zero;
            }

            var sum = invoices
                .Select(invoice => invoice.TimeElapsed)
                .Aggregate(TimeSpan.Zero, (timeElapsed1, timeElapsed2) => timeElapsed1 + timeElapsed2);
            
            return sum / invoices.Count;
        }
    }
}
