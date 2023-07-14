using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System.Globalization;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Validators
{
    internal class QuantityValidator : IQuantityValidator
    {
        public QuantityValidator(IHourValidator hourValidator, IDecimalValidator decimalValidator)
        {
            Guard.IsNotNull(hourValidator, nameof(hourValidator));
            Guard.IsNotNull(decimalValidator, nameof(decimalValidator));

            this.hourValidator = hourValidator;
            this.decimalValidator = decimalValidator;
        }

        public ValidationResult Validate(Annotation entity, CultureInfo cultureInfo, string fieldName)
        {
            Guard.IsNotNull(entity, nameof(entity));
            Guard.IsNotNull(entity.FieldValue, nameof(entity.FieldValue));

            if (entity.FieldValue.Contains(":"))
            {
                return hourValidator.Validate(entity, cultureInfo, fieldName);
            }
            else
            {
                return decimalValidator.Validate(entity, cultureInfo, fieldName);
            }
        }

        private readonly IHourValidator hourValidator;
        private readonly IDecimalValidator decimalValidator;
    }
}
