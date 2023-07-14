using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Services
{
    internal class DataAnotationComparisonService : IDataAnnotationComparisonService
    {
        public InvoiceRecognitionResult GetComparisonResult(DataAnnotation expectedAnnotation, DataAnnotation actualAnnotation)
        {
            var expectedAnnotations = GetExpectedAnnotations(expectedAnnotation);
            var totalFieldsCount = expectedAnnotations.Count;
            var correctlyAssignedFieldsCount = GetCorrectlyAssignedFieldsCount(expectedAnnotations, actualAnnotation.InvoiceAnnotations);
            var incorrectlyAssignedFieldsCount = actualAnnotation.InvoiceAnnotations.Count - correctlyAssignedFieldsCount;
            var lineItemsCount = expectedAnnotation.InvoiceLineAnnotations.Count;
            var lineItemsResult = GetLineItemAnnotation(expectedAnnotation.InvoiceLineAnnotations, actualAnnotation.InvoiceLineAnnotations);

            return new InvoiceRecognitionResult
            {
                TotalFieldsCount = totalFieldsCount,
                CorrectlyAssignedFieldsCount = correctlyAssignedFieldsCount,
                IncorrectlyAssignedFieldsCount = incorrectlyAssignedFieldsCount,
                LineItemsCount = lineItemsCount,
                FullyAssignedLineItemsCount = lineItemsResult.FullyAssignedCount,
                PartiallyAssignedLineItemsCount = lineItemsResult.PartiallyAssignedCount,
            };
        }

        private List<Annotation> GetExpectedAnnotations(DataAnnotation expectedAnnotation) 
        {
            return expectedAnnotation.InvoiceAnnotations
                .Where(annotation => annotation.DocumentLayoutItemIds.Count > 0)
                .ToList();
        }

        private int GetCorrectlyAssignedFieldsCount(List<Annotation> expectedAnnotationList, List<Annotation> actualAnnotationList)
        {
            var correctlyAssigned = 0;

            foreach (var expectedAnnotation in expectedAnnotationList)
            {
                var annotationToCheck = actualAnnotationList.FirstOrDefault(annotation => annotation.FieldType == expectedAnnotation.FieldType);

                if (annotationToCheck != null && annotationToCheck.FieldValue == expectedAnnotation.FieldValue)
                {
                    correctlyAssigned++;
                }
            }

            return correctlyAssigned;
        }

        private LineItemRecognitionResult GetLineItemAnnotation(List<LineAnnotation> expectedLineAnnotationList, List<LineAnnotation> actualLineAnnotationList)
        {
            var fullyAssignedCount = 0;

            foreach (var expectedLineAnnotation in expectedLineAnnotationList)
            {
                var actualLineAnnotation = actualLineAnnotationList.FirstOrDefault(lineAnnotation => lineAnnotation.OrderNumber == expectedLineAnnotation.OrderNumber);

                if (actualLineAnnotation == null)
                {
                    continue;
                }

                if (AreLinesEqual(expectedLineAnnotation, actualLineAnnotation))
                {
                    fullyAssignedCount++;
                    continue;
                }
            }

            return new LineItemRecognitionResult
            {
                FullyAssignedCount = fullyAssignedCount,
                PartiallyAssignedCount = actualLineAnnotationList.Count - fullyAssignedCount
            };
        }

        private bool AreLinesEqual(LineAnnotation expectedLineAnnotation, LineAnnotation actualLineAnnotation) 
        {
            return expectedLineAnnotation.LineItemAnnotations
                .OrderBy(item => item.FieldType)
                .SequenceEqual(
                    actualLineAnnotation.LineItemAnnotations.OrderBy(item => item.FieldType),
                    new AnnotationEqualityComparer());
        }
    }
}
