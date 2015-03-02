using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using RayTracer.ViewModel;

namespace RayTracer.Helpers.Converters
{
    public class TransformedPoint : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Vector4 point = (Vector4)values[0];
            Matrix3D matrix = (Matrix3D)values[1];

            return Transformations.TransformPoint(point, matrix);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
