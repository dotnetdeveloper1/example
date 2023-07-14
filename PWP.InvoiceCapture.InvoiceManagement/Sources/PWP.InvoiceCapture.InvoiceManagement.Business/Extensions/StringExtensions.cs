using System;
using System.Drawing.Imaging;
using System.Globalization;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Extensions
{
    internal static class StringExtensions
    {
        public static DateTime? ToNullableDate(this string value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return null;
            }

            return DateTime.Parse(value, cultureInfo, DateTimeStyles.None).Date;
        }

        public static DateTime? ToNullableDateTime(this string value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return null;
            }

            return DateTime.Parse(value, cultureInfo, DateTimeStyles.None);
        }

        public static decimal? ToNullableDecimal(this string value, CultureInfo cultureInfo) 
        {
            if (value == null)
            {
                return null;
            }

            return decimal.Parse(value, NumberStyles.Any, cultureInfo);
        }

        public static decimal FromHourOrDecimalToDecimal(this string value, CultureInfo cultureInfo)
        {
            if (value != null && value.Contains(":"))
            {
                var splitted = value.Split(':');

                var hours = splitted[0];
                var minutes = splitted[1];

                var convertedMinutes = Round(Convert.ToDecimal(minutes, cultureInfo) / 60);

                var convertedSeconds = 0m;
                if (splitted.Length > 2)
                {
                    convertedSeconds = Round(Convert.ToDecimal(splitted[2], cultureInfo) / 3600);
                }

                return Convert.ToDecimal(hours, cultureInfo) + convertedMinutes + convertedSeconds;
            }
            return value.ToNullableDecimal(cultureInfo) ?? 0;
        }

        public static ImageFormat ToImageFormat(this string imageFormat)
        {
            switch (imageFormat.ToLower())
            {
                case bmp:
                    return ImageFormat.Bmp;
                case tiff:
                    return ImageFormat.Tiff;
                case tif:
                    return ImageFormat.Tiff;
                case jpg:
                    return ImageFormat.Jpeg;
                case jpeg:
                    return ImageFormat.Jpeg;
                case png:
                    return ImageFormat.Png;
            }

            throw new ArgumentException($"Unknown image format: {imageFormat}.");
        }

        private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        private const string bmp = "bmp";
        private const string tiff = "tiff";
        private const string tif = "tif";
        private const string png = "png";
        private const string jpg = "jpg";
        private const string jpeg = "jpeg";
    }
}
