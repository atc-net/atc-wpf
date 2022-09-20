namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Thickness To Thickness binding.
/// </summary>
[ValueConversion(typeof(Thickness), typeof(Thickness), ParameterType = typeof(ThicknessSideType))]
public class ThicknessBindingValueConverter : IValueConverter
{
    public ThicknessSideType IgnoreThicknessSide { get; set; } = ThicknessSideType.None;

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Thickness thickness)
        {
            return default(Thickness);
        }

        var ignoreThickness = IgnoreThicknessSide;

        if (parameter is ThicknessSideType sideType)
        {
            ignoreThickness = sideType;
        }

        return ignoreThickness switch
        {
            ThicknessSideType.Left => new Thickness(0, thickness.Top, thickness.Right, thickness.Bottom),
            ThicknessSideType.Top => new Thickness(thickness.Left, 0, thickness.Right, thickness.Bottom),
            ThicknessSideType.Right => new Thickness(thickness.Left, thickness.Top, 0, thickness.Bottom),
            ThicknessSideType.Bottom => new Thickness(thickness.Left, thickness.Top, thickness.Right, 0),
            _ => thickness,
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}