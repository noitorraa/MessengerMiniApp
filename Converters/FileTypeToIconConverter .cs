using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp.Converters
{
    public class FileTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileType = value as string;
            if (fileType == null) return "icon_file.png";

            if (fileType.StartsWith("image/")) return "icon_image.png";
            if (fileType == "application/pdf") return "icon_pdf.png";
            if (fileType.StartsWith("audio/")) return "icon_audio.png";
            // ...другие кейсы
            return "icon_file.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
