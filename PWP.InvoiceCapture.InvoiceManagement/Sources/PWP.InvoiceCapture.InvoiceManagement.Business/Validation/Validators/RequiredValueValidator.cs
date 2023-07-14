using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class RequiredValueValidator : IRequiredValueValidator
    {
        public ValidationResult Validate(Annotation entity, string fieldName)
        {
            Guard.IsNotNull(entity, nameof(entity));
            Guard.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));

            var validationResult = string.IsNullOrWhiteSpace(entity.FieldValue)
                ? ValidationResult.Failed($"Field {fieldName} is empty.")
                : ValidationResult.Ok;

            return validationResult;
        }
    }
}
