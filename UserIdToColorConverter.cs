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
            if (value is int userId)
            {
                // Если сообщение отправлено текущим пользователем – один цвет, иначе – другой.
                return userId == CurrentUserId
                    ? Color.FromRgb(39, 167, 231)// Например, для своих сообщений
                    : Color.FromRgb(69, 89, 99);         // Для сообщений собеседника
            }
            return Color.FromRgb(244, 240, 246);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
