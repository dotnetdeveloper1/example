using System;
using System.Globalization;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract
{
    public static class DataTypeExtensions
    {   
        public static DataType GetDataType(this string value)
        {
            double numberType;
            var text = value;

            if(value.StartsWith("(") && value.EndsWith(")"))
            {
                text = value.Remove(0, 1);
                text = text.Remove(text.Length-1, 1);
            }

            string removedWhiteSpaces = text.RemoveWhiteSpaces();

            if (double.TryParse(removedWhiteSpaces.CleanCurrencyMarkers().GetDecimalValue(), out numberType))
            {
                return DataType.Number;
            }


            if (IsStringADate(value) || IsStringADate(removedWhiteSpaces))
            {
                return DataType.Date;
            }

            return DataType.String;
        }

        private static bool IsStringADate(string value)
        {
            DateTime dateType;
            foreach (var suffix in dateSuffixes)
            {
                value  = value.Replace(suffix, "");
            }

            value = value.Replace("-", "/").Replace(".", "/");

            if (DateTime.TryParseExact(value, dateTimeStyles,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out dateType))
            {
                return true;
            }
            if (DateTime.TryParseExact(value, additionalStyles,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out dateType))
            {
                return true;
            }
            if (DateTime.TryParseExact(value, customStyles,
               CultureInfo.InvariantCulture,
               DateTimeStyles.None, out dateType))
            {
                return true;
            }
            if (DateTime.TryParse(value, out dateType))
            {
                return true;
            }

            return false;
        }

        private static readonly string[] dateTimeStyles = CultureInfo.InvariantCulture.DateTimeFormat.GetAllDateTimePatterns();
        private static readonly string[] additionalStyles = new CultureInfo("en-AU").DateTimeFormat.GetAllDateTimePatterns();
        private static readonly string[] customStyles = new string[] { "d/M/yyyy", "M-d-yyyy", "M/d/yyyy", "dd/mm/yy", "mm/dd/yy" };
        private static readonly string[] dateSuffixes = new string[] { "nd","rd","st","th" };
    }
}
