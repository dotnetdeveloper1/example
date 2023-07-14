using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.Extensions
{
    public static class TelemetryClientExtensions
    {
        public static void TrackTrace(this ITelemetryClient client, string message, TelemetryData data) 
        {
            Guard.IsNotNull(client, nameof(client));
            Guard.IsNotNullOrWhiteSpace(message, nameof(message));
            Guard.IsNotNull(data, nameof(data));

            var properties = ToDictionary(data);

            client.TrackTrace(message, properties);
        }

        private static Dictionary<string, string> ToDictionary(object convertibleObject)
        {
            Guard.IsNotNull(convertibleObject, nameof(convertibleObject));

            var result = new Dictionary<string, string>();
            var properties = convertibleObject.GetType().GetProperties();

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(convertibleObject);
                var propertyName = property.Name;

                if (propertyValue != null)
                {
                    result.Add(propertyName, propertyValue.ToString());
                }
            }

            return result;
        }
    }
}
