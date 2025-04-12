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

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value switch
            {
                0 => Colors.Gray,    // Sent
                1 => Colors.Blue,    // Delivered
                2 => Colors.Green,   // Read
                _ => Colors.Gray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
