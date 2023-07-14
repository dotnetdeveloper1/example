using PWP.InvoiceCapture.Core.Utilities;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation
{
    public class ValidationResult
    {
        public bool IsValid => Message == null;
        public string Message { get; }

        public static ValidationResult Ok => new ValidationResult();
        public static ValidationResult Failed(string errorMessage) => new ValidationResult(errorMessage);

        private ValidationResult() { }
        private ValidationResult(string errorMessage)
        {
            Guard.IsNotNullOrWhiteSpace(errorMessage, nameof(errorMessage));
            Message = errorMessage;

        }
    }
}
