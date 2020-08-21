using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamChatLabelDecorator
{
    public class ChatLabelDecoratorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var chatdecoratorEngine = new ChatLabelDecoratorEngine();
            var formattedString = chatdecoratorEngine.GetFormattedString((string)value);
            return formattedString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
