using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MessengerMiniApp.Converters
{
    public class UserIdToAlignmentConverter : IValueConverter
    {
        // Если UserID == CurrentUserId — выравниваем вправо, иначе влево.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int userId && parameter is int currentUserId)
            {
                return userId == currentUserId
                    ? LayoutOptions.End
                    : LayoutOptions.Start;
            }
            return LayoutOptions.Start;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
