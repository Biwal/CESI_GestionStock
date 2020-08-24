using Models.Models;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Negosud.Converters
{
    class ProductQuantityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Product product = (Product) value;

            if(product.Quantity < product.MinStockAvailable)
            {
                return new SolidColorBrush(Color.FromArgb(255, 191, 64, 64));
            } else if (product.Quantity == product.MinStockAvailable) {
                return new SolidColorBrush(Color.FromArgb(255, 255, 112, 77));
            } else {
                return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
