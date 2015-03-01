using System.Windows.Data;

namespace RayTracer.Helpers.Converters
{
    public class ReducedValueConverter:IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double val = double.Parse(value.ToString());
            double delta = double.Parse(parameter.ToString());
            return val - delta;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
