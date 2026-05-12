// ReSharper disable InvertIf
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: multi-binding <c>(width, height)</c> → corner radius for an inscribed circle
/// (<c>min(width, height) / 2</c>). Use to make a rectangular control fully rounded into a pill or
/// circle that adapts to size changes.
/// </summary>
public sealed class RectangleCircularValueConverter : IMultiValueConverter
{
    public static readonly RectangleCircularValueConverter Instance = new();

    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
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

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}