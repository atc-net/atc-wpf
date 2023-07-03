namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: String Null Or Empty To Visibility-Visible.
/// </summary>
[ValueConversion(typeof(Thickness), typeof(double), ParameterType = typeof(ThicknessSideType))]
public sealed class ThicknessToDoubleValueConverter : IValueConverter
{
    public ThicknessSideType TakeThicknessSide { get; set; } = ThicknessSideType.None;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Thickness thickness)
        {
            return default(double);
        }

        var takeThicknessSide = TakeThicknessSide;

        if (parameter is ThicknessSideType sideType)
        {
            takeThicknessSide = sideType;
        }

        return takeThicknessSide switch
        {
            ThicknessSideType.Left => thickness.Left,
            ThicknessSideType.Top => thickness.Top,
            ThicknessSideType.Right => thickness.Right,
            ThicknessSideType.Bottom => thickness.Bottom,
            _ => default,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}