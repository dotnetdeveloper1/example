using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class MaxValueValidator : IMaxValueValidator
    {
        public ValidationResult Validate(Annotation entity, decimal maxValue, string fieldName)
        {
            Guard.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));
            Guard.IsNotNull(entity, nameof(entity));
            Guard.IsNotNull(entity.FieldValue, nameof(entity.FieldValue));

            var value = decimal.Parse(entity.FieldValue);

            var validationResult = value > maxValue
                    ? ValidationResult.Failed($"Field {fieldName} is more than {maxValue}.")
                    : ValidationResult.Ok;

            return validationResult;
        }
    }
}
