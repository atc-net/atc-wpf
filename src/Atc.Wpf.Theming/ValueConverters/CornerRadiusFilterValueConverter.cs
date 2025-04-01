namespace Atc.Wpf.Theming.ValueConverters;

[ValueConversion(typeof(CornerRadius), typeof(CornerRadius), ParameterType = typeof(RadiusType))]
[ValueConversion(typeof(CornerRadius), typeof(double), ParameterType = typeof(RadiusType))]
public sealed class CornerRadiusFilterValueConverter : IValueConverter
{
    public static readonly CornerRadiusFilterValueConverter Instance = new();

    public RadiusType Filter { get; set; } = RadiusType.None;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CornerRadius cornerRadius)
        {
            return Binding.DoNothing;
        }

        var filter = Filter;
        if (parameter is RadiusType radiusType)
        {
            filter = radiusType;
        }

        return filter switch
        {
            RadiusType.Left => new CornerRadius(cornerRadius.TopLeft, 0, 0, cornerRadius.BottomLeft),
            RadiusType.Top => new CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, 0, 0),
            RadiusType.Right => new CornerRadius(0, cornerRadius.TopRight, cornerRadius.BottomRight, 0),
            RadiusType.Bottom => new CornerRadius(0, 0, cornerRadius.BottomRight, cornerRadius.BottomLeft),
            RadiusType.TopLeft => cornerRadius.TopLeft,
            RadiusType.TopRight => cornerRadius.TopRight,
            RadiusType.BottomRight => cornerRadius.BottomRight,
            RadiusType.BottomLeft => cornerRadius.BottomLeft,
            _ => cornerRadius,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}