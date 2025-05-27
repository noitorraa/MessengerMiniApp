using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp
{
    public class UserIdToColorConverter : IValueConverter
    {
        /// <summary>
        /// Текущий ID пользователя, который отправляет сообщения.
        /// </summary>
        public int CurrentUserId { get; set; }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (int)value switch
            {
                0 => Colors.Gray,    // Gray for "sent"
                1 => Colors.Blue,    // Blue for "delivered"
                2 => Colors.Green,   // Green for "read"
                _ => Colors.Red
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for this converter");
        }
    }
}
