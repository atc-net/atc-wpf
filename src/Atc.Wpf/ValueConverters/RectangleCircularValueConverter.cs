// ReSharper disable InvertIf
namespace Atc.Wpf.ValueConverters;

public sealed class RectangleCircularValueConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not null &&
            values.Length == 2 &&
            values[0] is double width &&
            values[1] is double height)
        {
            if (width < double.Epsilon || height < double.Epsilon)
            {
                return .0;
            }

            var min = System.Math.Min(width, height);
            return min / 2;
        }

        return DependencyProperty.UnsetValue;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}