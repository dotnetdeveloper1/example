using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System.Globalization;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class HourValidator : IHourValidator
    {
        public ValidationResult Validate(Annotation entity, CultureInfo cultureInfo, string fieldName)
        {
            Guard.IsNotNull(entity, nameof(entity));
            Guard.IsNotNull(entity.FieldValue, nameof(entity.FieldValue));
            Guard.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));

            var splittedValues = entity.FieldValue.Split(':');
            var failedResult = ValidationResult.Failed($"Field {fieldName} can't be converted to hour.");
           
            if (splittedValues.Length > 3) 
            { 
                return failedResult;
            }

            foreach (var splittedValue in splittedValues)
            {
                if (!ValidateTimePart(splittedValue))
                {
                    return failedResult;
                }
            }

            return ValidationResult.Ok;
        }

        private bool ValidateTimePart(string value)
        {
            if (int.TryParse(value, out var parsedInt))
            {
                if (parsedInt < 0 || parsedInt >= 60)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
