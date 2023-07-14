using NCalc;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class FormulaExecutionResultValidator : IFormulaExecutionResultValidator
    {
        public FormulaExecutionResultValidator(IFormulaExtractionService formulaExtractionService) 
        {
            Guard.IsNotNull(formulaExtractionService, nameof(formulaExtractionService));

            this.formulaExtractionService = formulaExtractionService;
        }

        public ValidationResult Validate(List<Annotation> annotations, Annotation resultAnnotation, string resultFieldName, string formula) 
        {
            Guard.IsNotNullOrEmpty(annotations, nameof(annotations));
            Guard.IsNotNull(resultAnnotation, nameof(resultAnnotation));
            Guard.IsNotNullOrWhiteSpace(resultFieldName, nameof(resultFieldName));
            Guard.IsNotNullOrWhiteSpace(formula, nameof(formula));

            var operandIds = formulaExtractionService.GetFieldIds(formula);
            var annotationsDictionary = annotations.ToDictionary(annotation => int.Parse(annotation.FieldType), annotation => annotation.FieldValue);
            var keyValues = new Dictionary<int, decimal>();

            foreach (var operandId in operandIds)
            {
                var value = annotationsDictionary.ContainsKey(operandId)
                    ? decimal.Parse(annotationsDictionary[operandId])
                    : 0;

                keyValues.Add(operandId, Round(value));
            }

            var normalizedFormula = formulaExtractionService.GetNormalizedFormula(formula, keyValues);
            var expression = new Expression(normalizedFormula);
            var executedResult = expression.Evaluate().ToString();
            var executedValue = decimal.Parse(executedResult);
            var resultAnnotationValue = decimal.Parse(resultAnnotation.FieldValue);

            return Round(executedValue) == Round(resultAnnotationValue)
                ? ValidationResult.Ok
                : ValidationResult.Failed($"Formula execution result of {resultFieldName} field is not equal to its value.");
        }

        private decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        private readonly IFormulaExtractionService formulaExtractionService;
    }
}
