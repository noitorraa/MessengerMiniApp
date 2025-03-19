using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp
{
    public class FileTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string fileType || parameter is not string type)
                return Visibility.Collapsed;

            var isImage = fileType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

            return type switch
            {
                "image" when isImage => Visibility.Visible,
                "other" when !isImage => Visibility.Visible,
                _ => Visibility.Collapsed
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
