using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System;
using System.Drawing;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class CalculationService: ICalculationService
    {
        public int CalculateHeight(int newWidth, SizeF previousSize)
        {
            Guard.IsNotZeroOrNegative(newWidth, nameof(newWidth));
            GuardSize(previousSize);

            var scale = newWidth / previousSize.Width;

            var newHeight = Convert.ToInt32(previousSize.Height * scale);

            return newHeight;
        }

        //TODO: move to Core
        private void GuardSize(SizeF size)
        {
            if (size.Width <= 0)
            {
                throw new ArgumentException($"{nameof(size.Width)} is an invalid value. Should be greater than zero.");
            }

            if (size.Height <= 0)
            {
                throw new ArgumentException($"{nameof(size.Height)} is an invalid value. Should be greater than zero.");
            }
        }
    }
}
