// Converters/StatusToColorConverter.cs
using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using MessengerServer.Models;

namespace MessengerMiniApp.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        // Преобразует MessageStatus (enum) или int-статус в цвет (например, серый для «отправлено», зелёный для «прочитано»).
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Если value — int
            if (value is int i)
            {
                return i switch
                {
                    0 => Colors.LightGray,
                    1 => Colors.Gray,
                    2 => Colors.Green,
                    _ => Colors.LightGray,
                };
            }
            return Colors.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
