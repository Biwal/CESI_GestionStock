using Models.Models;
using Models.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace Negosud.Converters
{
    class OrderStatusEnumConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is Enum))
                return null;

            var @enum = value as Enum;
            var description = @enum.ToString();

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotImplementedException();
        }

        private T GetAttribute<T>(Enum enumValue) where T : Attribute
        {
            return enumValue.GetType().GetTypeInfo()
                .GetDeclaredField(enumValue.ToString())
                .GetCustomAttribute<T>();
        }
    }
}
