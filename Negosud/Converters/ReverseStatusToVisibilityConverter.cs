using Models.Utils;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Negosud.Converters
{
    public class ReverseStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is OrderStatus))
                return Visibility.Collapsed;
            OrderStatus orderStatus = (OrderStatus)value;
            return orderStatus == OrderStatus.NONE ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }
}
