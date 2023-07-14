using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.Core.Utilities
{
    public static class Guard
    {
        public static void IsNotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void IsNotNullOrWhiteSpace(string argument, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void IsNotZeroOrNegative(int argument, string argumentName)
        {
            if (argument <= 0)
            {
                throw new ArgumentException($"{argumentName} is an invalid value. Should be greater than zero.");
            }
        }

        public static void IsNotNullOrEmpty<T>(IEnumerable<T> argument, string argumentName)
        {
            if (argument == null || !argument.Any())
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void IsEnumDefined<T> (T value, string parameterName) where T : struct
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                throw new ArgumentException("Invalid enum value.", parameterName);
            }
        }
    }
}
