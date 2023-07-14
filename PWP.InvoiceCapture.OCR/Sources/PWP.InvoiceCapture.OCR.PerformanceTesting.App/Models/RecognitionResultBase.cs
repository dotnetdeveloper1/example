namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models
{
    internal class RecognitionResultBase
    {
        protected double GetPercent(double x, double y)
        {
            if (y == 0)
            {
                return 0;
            }

            return x / y;
        }
    }
}
