// Converters/NullToVisibilityConverter.cs
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MessengerMiniApp.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Если value == null или пустая строка — скрыть (return false для IsVisible)
            if (value == null) return false;
            if (value is string s && string.IsNullOrWhiteSpace(s)) return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
