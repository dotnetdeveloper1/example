using System.ComponentModel.DataAnnotations;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Attributes
{
    //TODO: Move to Core package
    public class MinValueAttribute : ValidationAttribute
    {
        public MinValueAttribute(int minValue) 
        {
            this.minValue = minValue;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            var intValue = (int)value;

            return intValue >= minValue;
        }

        private readonly int minValue;
    }
}
