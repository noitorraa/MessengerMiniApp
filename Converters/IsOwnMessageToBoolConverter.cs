using MessengerMiniApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp.Converters
{
    public class IsOwnMessageToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // параметр parameter передаём как self на страницу, чтобы из BindingContext взять currentUserId
            var page = (BindableObject)parameter;
            var vm = (ChatViewModel)page.BindingContext;
            int currentUserId = vm.CurrentUserId;

            int messageUserId = (int)value;
            return messageUserId == currentUserId;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
