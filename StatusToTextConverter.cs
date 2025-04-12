using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp
{
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine($"Получено значение статуса: {value} (тип: {value?.GetType()})");

            if (value is int status)
            {
                return status switch
                {
                    0 => "✓",      // Sent
                    1 => "✓✓",     // Delivered
                    2 => "✓✓✓",    // Read
                    _ => "✓"       // Fallback
                };
            }

            // Логирование ошибки
            Console.WriteLine($"Некорректный тип данных: {value?.GetType()}");
            return "✓";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
