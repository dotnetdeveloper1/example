using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract
{
    public static class StringExtensions
    {
        // Get decimal value of a string, NB: this method will only work on numerical strings, do the cleaning before passing any values into it.
        public static string GetDecimalValue(this string input)
        {
            var isZero = string.Equals(input.RemoveWhiteSpaces(), "0");
            if (isZero)
            {
                return input;
            }

            char decimalChar;
            char thousandsChar;

            // Try to guess which character is used for decimals and which is used for thousands
            if (input.LastIndexOf(',') > input.LastIndexOf('.'))
            {
                decimalChar = ',';
                thousandsChar = '.';
            }
            else
            {
                decimalChar = '.';
                thousandsChar = ',';
            }

            // Remove thousands separators as they are not needed for parsing
            var result = input.Replace(thousandsChar.ToString(), string.Empty);

            // Replace decimal separator with the one from InvariantCulture
            // This makes sure the decimal parses successfully using InvariantCulture
            result = result.Replace(decimalChar.ToString(),
                CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator);

            if (decimal.TryParse(result, NumberStyles.AllowDecimalPoint | NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalResult))
            {
                return result.GetLeadingCharPart('0') + decimalResult.ToString();
            }

            return input;
        }

        public static string GetLeadingCharPart(this string input, char charToSearch)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            int currentIndex = 0;
            char currentChar = input[currentIndex];       
            var leadingZeros = new List<char>();
            
            while(currentChar.Equals(charToSearch))
            {
                leadingZeros.Add(charToSearch);
                currentIndex++;
                
                if(currentIndex == input.Length)
                {
                    break;
                } 
                
                currentChar = input[currentIndex];
            }

            return string.Join("", leadingZeros);
        }

        public static string GetAnnotatedValue(this string input, bool isNumberExpected)
        {
            var replacedInput = isNumberExpected ? ReplaceDashCharacterToZero(input) : input;

            if (replacedInput.GetDataType() == DataType.Number)
            {
                var result = replacedInput.RemoveWhiteSpaces();
                
                // Simple replace of all non numeric and non ',' '.' '-' characters to account for numeric formats
                result = String.Join("", result.Where(character => char.IsDigit(character) || character == ',' || character == '.' || character == '-'));
                var decimalValue = result.GetDecimalValue();
                
                if(input.StartsWith("(") && input.EndsWith(")"))
                {
                    decimalValue = "-" + decimalValue;
                }
                
                return decimalValue;
            }

            return input;
        }

        public static string GetDescription(this Enum genericEnum) 
        {
            var fieldInfo = genericEnum.GetType().GetField(genericEnum.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return (attributes != null && attributes.Any())
                ? attributes.First().Description
                : genericEnum.ToString();
        }

        public static string GetDigest(this string input)
        {
            return new string(input.ToLower().Where(c => !vowels.Contains(c) && char.IsLetterOrDigit(c)).ToArray());
        }

        public static string GetAlphaNumericValue(this string input)
        {
            return new string(input.ToLower().Where(c => char.IsLetterOrDigit(c)).ToArray());
        }

        public static string GetIdentifier(this string input)
        {
            return new String(input.GetDigest().ToArray());
        }

        public static string GetNumberValue(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }   

            return new string(input.Replace(" ", "").Where(c => char.IsDigit(c) || c.Equals('.')).ToArray());
        }

        public static string CleanCurrencyMarkers(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            foreach(var currency in currencyMarkers)
            {
                input = input.Replace(currency, "");
            }

            return input;
        }

        public static string RemoveWhiteSpaces(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.Replace(" ", "");
        }

        private static string ReplaceDashCharacterToZero(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var modifiedText = text.RemoveWhiteSpaces().Replace("-", string.Empty);

            if (string.IsNullOrEmpty(modifiedText))
            {
                return "0";
            }

            return text;
        }

        private static readonly char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y', ' ' };
        private static readonly string[] currencyMarkers = new string[] { "US$","USD","$","US","S","AUD","EUR", "€" };
        
    }
}
