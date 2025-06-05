using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp.Converters
{
    public class UserIdToColorConverter : IValueConverter
    {
        /// <summary>
        /// Текущий ID пользователя, который отправляет сообщения.Add commentMore actions
        /// </summary>
        public int CurrentUserId { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int userId)
            {
                // Если сообщение отправлено текущим пользователем – один цвет, иначе – другой.
                return userId == CurrentUserId
                    ? Color.FromArgb("#FEAAAA") // фиолетовый для своих
                    : Color.FromArgb("#EAEAEA"); // синий для чужих
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
