using System;
using System.ComponentModel.DataAnnotations;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Attributes
{
    //TODO: Move to Core package
    public class EnumTypeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            var type = value.GetType();

            if (type.IsEnum)
            {
                return Enum.IsDefined(type, value);
            }

            return false;
        }
    }
}
