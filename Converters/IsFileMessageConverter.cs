using System.Globalization;
using MessengerMiniApp.DTOs;

namespace MessengerMiniApp.Converters
{
    public class IsFileMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is MessageDto message && !string.IsNullOrEmpty(message.FileUrl);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}