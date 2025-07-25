// Converters/NullToColorConverter.cs
using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace MessengerMiniApp.Converters
{
    public class NullToColorConverter : IValueConverter
    {
        // Если Avatar == null или пустая строка → возвращаем серый фон
        // иначе — возвращаем Transparent (или иной «фон позади картинки»)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
                return Colors.Transparent;
            return Color.FromArgb("#CCCCCC");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
