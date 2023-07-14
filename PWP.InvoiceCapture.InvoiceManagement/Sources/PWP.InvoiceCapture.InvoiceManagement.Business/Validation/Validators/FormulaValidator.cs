using NCalc;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class FormulaValidator : IFormulaValidator
    {
        public FormulaValidator(IFormulaExtractionService formulaExtractionService)
        {
            Guard.IsNotNull(formulaExtractionService, nameof(formulaExtractionService));

            this.formulaExtractionService = formulaExtractionService;
        }

        public ValidationResult Validate(List<Field> fields, Field currentField, bool isFieldUsedAsOperand)
        {
            Guard.IsNotNull(fields, nameof(fields));

            if (currentField.Type != FieldType.Decimal && isFieldUsedAsOperand)
            {
                return ValidationResult.Failed("Can't change field type. Field is used as operand in other field validation formula.");
            }

            if (string.IsNullOrEmpty(currentField.Formula))
            {
                return ValidationResult.Ok;
            }

            if (currentField.Type != FieldType.Decimal)
            {
                return ValidationResult.Failed("Can't set validation formula for non-decimal field.");
            }

            if (!formulaExtractionService.AreSquareBracketsBalanced(currentField.Formula))
            {
                return ValidationResult.Failed("Invalid validation formula. Square brackets is not balanced.");
            }

            var fieldsIds = formulaExtractionService.GetFieldIds(currentField.Formula);

            if (fieldsIds == null)
            {
                return ValidationResult.Failed("Invalid validation formula. Can't parse field id.");
            }

            var existingFields = fields.Where(field => fieldsIds.Any(fieldId => field.Id == fieldId)).ToList();

            if (currentField.Formula.Contains('/'))
            {
                return ValidationResult.Failed("Invalid validation formula. Division operation is not allowed.");
            }

            if (existingFields.Count != fieldsIds.Count)
            {
                return ValidationResult.Failed("Invalid validation formula. Field with specified id(s) doesn't exist.");
            }

            if (existingFields.Any(field => field.Type != FieldType.Decimal))
            {
                return ValidationResult.Failed("Invalid validation formula. The type of one or more formula fields is different from decimal.");
            }
            var fieldDefaultValues = fields.ToDictionary(field => field.Id, field => (decimal)defaultFieldValue);
            var normalizedFormula = formulaExtractionService.GetNormalizedFormula(currentField.Formula, fieldDefaultValues);
            var expression = new Expression(normalizedFormula);

            if (expression.HasErrors())
            {
                return ValidationResult.Failed("Invalid validation formula. Can't calculate formula expression.");
            }

            return ValidationResult.Ok;
        }

        private const int defaultFieldValue = 1;

        private readonly IFormulaExtractionService formulaExtractionService;
    }
}
