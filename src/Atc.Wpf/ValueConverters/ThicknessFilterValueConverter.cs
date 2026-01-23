namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Thickness To Thickness with filter.
/// </summary>
[ValueConversion(typeof(Thickness), typeof(Thickness), ParameterType = typeof(LeftTopRightBottomType))]
public sealed class ThicknessFilterValueConverter : IValueConverter
{
    public static readonly ThicknessFilterValueConverter Instance = new();

    public LeftTopRightBottomType Filter { get; set; } = LeftTopRightBottomType.None;

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not Thickness thickness)
        {
            return Binding.DoNothing;
        }

        var filter = Filter;

        if (parameter is LeftTopRightBottomType sideType)
        {
            filter = sideType;
        }

        return filter switch
        {
            LeftTopRightBottomType.Left => new Thickness(thickness.Left, 0, 0, 0),
            LeftTopRightBottomType.Top => new Thickness(0, thickness.Top, 0, 0),
            LeftTopRightBottomType.Right => new Thickness(0, 0, thickness.Right, 0),
            LeftTopRightBottomType.Bottom => new Thickness(0, 0, 0, thickness.Bottom),
            _ => thickness,
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => DependencyProperty.UnsetValue;
}