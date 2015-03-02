using System;
using System.Globalization;
using System.Windows.Data;

namespace RayTracer.Helpers.Converters
{
    public class ReducedValueConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = double.Parse(value.ToString());
            double delta = double.Parse(parameter.ToString());
            return val - delta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
