using Microsoft.Maui.Controls;
using System.Globalization;
using MessengerMiniApp.Pages;
using MessengerMiniApp.ViewModels; // Добавьте это пространство имён

namespace MessengerMiniApp.Converters
{
    public class UserIdToColorConverter : IValueConverter
    {
        // UserIdToColorConverter.cs
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int userId && parameter is ContentPage page)
            {
                if (page.BindingContext is ChatViewModel vm)
                {
                    return userId == vm.CurrentUserId
                        ? Color.FromArgb("#FEAAAA")
                        : Color.FromArgb("#EAEAEA");
                }
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}