using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using Microsoft.Maui.Graphics;

namespace MessengerMiniApp;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int status)
        {
            return status switch
            {
                0 => Colors.Black,   // Серый для "отправлено"
                1 => Colors.Blue,   // Синий для "доставлено"
                2 => Colors.Red,  // Зеленый для "прочитано"
                _ => Colors.Black    // Дефолтный цвет
            };
        }
        return Colors.Gray; // На случай ошибок
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
