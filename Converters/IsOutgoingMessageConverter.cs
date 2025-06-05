using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp.Converters
{
    public class IsOutgoingMessageConverter : IValueConverter
    {
        /// <summary>
        /// Current user ID for determining outgoing messages
        /// </summary>
        public int CurrentUserId { get; set; }

        /// <summary>
        /// Converts a user ID to a boolean indicating if the message is outgoing
        /// </summary>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int userId)
            {
                // Message is outgoing if the user ID matches the current user
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
