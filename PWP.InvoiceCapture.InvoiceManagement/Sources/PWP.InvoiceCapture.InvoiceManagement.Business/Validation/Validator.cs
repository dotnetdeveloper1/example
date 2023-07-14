using PWP.InvoiceCapture.Core.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation
{
    public static class Validator
    {
        public static async Task<ValidationResult> ValidateManyAsync(params Task<ValidationResult>[] validationActions)
        {
            Guard.IsNotNull(validationActions, nameof(validationActions));

            var validationResults = await Task.WhenAll(validationActions);

            return ValidateMany(validationResults);
        }

        public static async Task<ValidationResult> ValidateManyAsync(List<Task<ValidationResult>> validationActions)
        {
            Guard.IsNotNull(validationActions, nameof(validationActions));

            var validationResults = await Task.WhenAll(validationActions);

            return ValidateMany(validationResults);
        }

        public static ValidationResult ValidateMany(params ValidationResult[] validationResults)
        {
            Guard.IsNotNull(validationResults, nameof(validationResults));

            return validationResults.Combine();
        }
    }
}
