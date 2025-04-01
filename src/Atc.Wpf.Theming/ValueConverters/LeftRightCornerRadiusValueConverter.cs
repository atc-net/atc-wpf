namespace Atc.Wpf.Theming.ValueConverters;

[ValueConversion(typeof(CornerRadius), typeof(CornerRadius))]
public sealed class LeftRightCornerRadiusValueConverter : IValueConverter
{
    public static readonly LeftRightCornerRadiusValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not CornerRadius globalRadius)
        {
            return new CornerRadius(0);
        }

        var r = globalRadius.TopLeft;
        return new CornerRadius(0, r, r, 0);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}