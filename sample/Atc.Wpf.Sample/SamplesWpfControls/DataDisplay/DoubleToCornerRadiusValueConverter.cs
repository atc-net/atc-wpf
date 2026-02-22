namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

[ValueConversion(typeof(double), typeof(CornerRadius))]
internal sealed class DoubleToCornerRadiusValueConverter : IValueConverter
{
    public static readonly DoubleToCornerRadiusValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is double d
            ? new CornerRadius(d)
            : new CornerRadius(0);

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is CornerRadius cr
            ? cr.TopLeft
            : 0.0;
}