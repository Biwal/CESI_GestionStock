using Models.Utils;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Negosud.Converters
{
    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
        
            if (value == null || !(value is OrderStatus))
                return Visibility.Collapsed;
            OrderStatus orderStatus = (OrderStatus)value;
            return orderStatus == OrderStatus.NONE ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }
}
