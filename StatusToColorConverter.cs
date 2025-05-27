using System.Globalization;

namespace MessengerMiniApp;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int status)
        {
            return status switch
            {
                0 => Colors.Gray,    // Gray for "sent"
                1 => Colors.Blue,    // Blue for "delivered"
                2 => Colors.Green,   // Green for "read"
                _ => Colors.Black    // Дефолтный цвет
            };
        }
        return Colors.Red; // На случай ошибок
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
