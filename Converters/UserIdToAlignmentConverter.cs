using Microsoft.Maui.Controls;
using System.Globalization;
using MessengerMiniApp.Pages;
using MessengerMiniApp.ViewModels; // Добавьте это пространство имён

using System.Globalization;
using Microsoft.Maui.Controls;

namespace MessengerMiniApp.Converters
{
    public class UserIdToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int userId && parameter is ChatPage page)
            {
                if (page.BindingContext is ChatViewModel viewModel)
                {
                    // Если это сообщение текущего пользователя - выравниваем по правому краю
                    return userId == viewModel.CurrentUserId ? LayoutOptions.End : LayoutOptions.Start;
                }
            }
            return LayoutOptions.Start;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}