namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Thickness To Thickness binding.
/// </summary>
[ValueConversion(typeof(Thickness), typeof(Thickness), ParameterType = typeof(LeftTopRightBottomType))]
public sealed class ThicknessBindingValueConverter : IValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="ThicknessBindingValueConverter"/>.
    /// </summary>
    public static readonly ThicknessBindingValueConverter Instance = new();

    /// <summary>
    /// Gets or sets the side whose thickness is set to zero when no side is passed as the converter parameter.
    /// </summary>
    public LeftTopRightBottomType IgnoreThicknessSide { get; set; } = LeftTopRightBottomType.None;

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not Thickness thickness)
        {
            return default(Thickness);
        }

        var ignoreThickness = IgnoreThicknessSide;

        if (parameter is LeftTopRightBottomType sideType)
        {
            ignoreThickness = sideType;
        }

        return ignoreThickness switch
        {
            LeftTopRightBottomType.Left => new Thickness(0, thickness.Top, thickness.Right, thickness.Bottom),
            LeftTopRightBottomType.Top => new Thickness(thickness.Left, 0, thickness.Right, thickness.Bottom),
            LeftTopRightBottomType.Right => new Thickness(thickness.Left, thickness.Top, 0, thickness.Bottom),
            LeftTopRightBottomType.Bottom => new Thickness(thickness.Left, thickness.Top, thickness.Right, 0),
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