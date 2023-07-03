namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Thickness To Thickness with filter.
/// </summary>
[ValueConversion(typeof(Thickness), typeof(Thickness), ParameterType = typeof(ThicknessSideType))]
public sealed class ThicknessFilterValueConverter : IValueConverter
{
    public ThicknessSideType Filter { get; set; } = ThicknessSideType.None;

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Thickness thickness)
        {
            return Binding.DoNothing;
        }

        var filter = Filter;

        if (parameter is ThicknessSideType sideType)
        {
            filter = sideType;
        }

        return filter switch
        {
            ThicknessSideType.Left => new Thickness(thickness.Left, 0, 0, 0),
            ThicknessSideType.Top => new Thickness(0, thickness.Top, 0, 0),
            ThicknessSideType.Right => new Thickness(0, 0, thickness.Right, 0),
            ThicknessSideType.Bottom => new Thickness(0, 0, 0, thickness.Bottom),
            _ => thickness,
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}