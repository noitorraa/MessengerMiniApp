using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp
{
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int status)
            {
                return status switch
                {
                    0 => "Sent",      // Sent status
                    1 => "Delivered", // Delivered status
                    2 => "Read",      // Read status
                    _ => "Unknown"    // Fallback for unknown status
                };
            }

            return "Unknown";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for this converter");
        }
    }
}
