using System;
using System.Collections.Generic;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Extensions
{
    internal static class EnumExtensions
    {
        public static Dictionary<string, TValue> GetKeyValues<T, TValue>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T)).Cast<TValue>();

            return values.ToDictionary(value => Enum.GetName(typeof(T), value), value => value);
        }
    }
}
