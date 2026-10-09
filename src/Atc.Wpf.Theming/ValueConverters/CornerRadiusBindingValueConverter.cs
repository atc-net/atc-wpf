namespace Atc.Wpf.Theming.ValueConverters;

/// <summary>
/// ValueConverter: zeroes the selected side(s) of a <see cref="CornerRadius"/> and passes the
/// remaining corners through. The side is picked via <c>ConverterParameter</c>
/// (a <see cref="RadiusType"/>) or the <see cref="IgnoreRadius"/> property.
/// </summary>
[ValueConversion(typeof(CornerRadius), typeof(CornerRadius), ParameterType = typeof(RadiusType))]
public sealed class CornerRadiusBindingValueConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared instance of the converter.
    /// </summary>
    public static readonly CornerRadiusBindingValueConverter Instance = new();

    /// <summary>
    /// Gets or sets the side(s) to zero when no <see cref="RadiusType"/> converter parameter is given.
    /// </summary>
    public RadiusType IgnoreRadius { get; set; } = RadiusType.None;

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not CornerRadius cornerRadius)
        {
            return default(CornerRadius);
        }

        var ignoreRadius = IgnoreRadius;
        if (parameter is RadiusType radiusType)
        {
            ignoreRadius = radiusType;
        }

        return ignoreRadius switch
        {
            RadiusType.Left => new CornerRadius(0, cornerRadius.TopRight, cornerRadius.BottomRight, 0),
            RadiusType.Top => new CornerRadius(0, 0, cornerRadius.BottomRight, cornerRadius.BottomLeft),
            RadiusType.Right => new CornerRadius(cornerRadius.TopLeft, 0, 0, cornerRadius.BottomLeft),
            RadiusType.Bottom => new CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, 0, 0),
            RadiusType.TopLeft => new CornerRadius(0, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft),
            RadiusType.TopRight => new CornerRadius(cornerRadius.TopLeft, 0, cornerRadius.BottomRight, cornerRadius.BottomLeft),
            RadiusType.BottomRight => new CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, 0, cornerRadius.BottomLeft),
            RadiusType.BottomLeft => new CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, 0),
            _ => cornerRadius,
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