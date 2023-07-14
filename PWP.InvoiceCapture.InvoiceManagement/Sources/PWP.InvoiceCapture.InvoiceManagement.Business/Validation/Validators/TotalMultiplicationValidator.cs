using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Extensions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class TotalMultiplicationValidator : ITotalMultiplicationValidator
    {
        public ValidationResult Validate(Annotation[] itemsToMultiply, Annotation total, CultureInfo cultureInfo, Dictionary<string, string> fieldTypeNames)
        {
            Guard.IsNotNull(fieldTypeNames, nameof(fieldTypeNames));
            Guard.IsNotNull(itemsToMultiply, nameof(itemsToMultiply));
            Guard.IsNotNull(total, nameof(total));

            var decimalsToMultiplicate = itemsToMultiply
                .Where(item => item != null)
                .Select(item => ConvertAnnotationValueToDecimal(item, cultureInfo))
                .ToList();

            if (decimalsToMultiplicate.Count < 2)
            {
                return ValidationResult.Ok;
            }

            var multiplication = decimalsToMultiplicate.Aggregate((operand1, operand2) => 
                operand1 * operand2);

            return Round(multiplication) == Round(ConvertAnnotationValueToDecimal(total, cultureInfo))
               ? ValidationResult.Ok
               : ValidationResult.Failed($"{GetErrorMessage(itemsToMultiply, fieldTypeNames)}");
        }

        private string GetErrorMessage(Annotation[] annotations, Dictionary<string, string> fieldTypeNames)
        {
            var fieldNamesToMultiply = fieldTypeNames
                .Where(field => annotations
                    .Any(annotation => annotation != null && string.Equals(annotation.FieldType.ToLower(), field.Key.ToLower())))
                .Select(field => field.Value)
                .ToList();

            return $"Total is not equal to multiplication of {string.Join(",", fieldNamesToMultiply)}.";
        }

        private decimal ConvertAnnotationValueToDecimal(Annotation annotation, CultureInfo cultureInfo)
        {
            return annotation.FieldValue.FromHourOrDecimalToDecimal(cultureInfo);
        }
        
        private decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
