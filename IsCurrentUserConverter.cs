using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp
{
    public class IsCurrentUserConverter : IValueConverter
    {
        /// <summary>
        /// Current user ID for determining if content belongs to the current user
        /// </summary>
        public int CurrentUserId { get; set; }

        /// <summary>
        /// Converts a user ID to a boolean indicating if it matches the current user
        /// </summary>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int userId)
            {
                // Content belongs to current user if IDs match
                return userId == CurrentUserId;
            }
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for this converter");
        }
    }
}
