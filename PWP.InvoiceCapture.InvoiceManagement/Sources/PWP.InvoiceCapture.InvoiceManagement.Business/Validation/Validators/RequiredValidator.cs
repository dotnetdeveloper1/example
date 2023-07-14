using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class RequiredValidator : IRequiredValidator
    {
        public ValidationResult Validate(Annotation annotation, string fieldName)
        {
            Guard.IsNotNullOrEmpty(fieldName, nameof(fieldName));

            var validationResult = annotation == null
                ? ValidationResult.Failed($"Missing annotation for {fieldName}.")
                : ValidationResult.Ok;

            return validationResult;
        }
    }

}
