using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class MinValueValidator : IMinValueValidator
    {
        public ValidationResult Validate(Annotation entity, decimal minValue, string fieldName)
        {
            Guard.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));
            Guard.IsNotNull(entity, nameof(entity));
            Guard.IsNotNull(entity.FieldValue, nameof(entity.FieldValue));

            var value = decimal.Parse(entity.FieldValue);

            var validationResult = value < minValue
                    ? ValidationResult.Failed($"Field {fieldName} is less than {minValue}.")
                    : ValidationResult.Ok;

            return validationResult;
        }
    }
}
