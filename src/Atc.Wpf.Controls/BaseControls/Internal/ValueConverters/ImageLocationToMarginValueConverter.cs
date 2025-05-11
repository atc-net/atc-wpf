namespace Atc.Wpf.Controls.BaseControls.Internal.ValueConverters;

/// <summary>
/// ValueConverter: ImageLocation to Thickness (Margin).
/// </summary>
[ValueConversion(typeof(ImageLocation), typeof(Thickness))]
internal sealed class ImageLocationToMarginValueConverter : IValueConverter
{
    public static readonly ImageLocationToMarginValueConverter Instance = new();

    public double Spacing { get; set; } = 5.0;

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not ImageLocation imageLocation)
        {
            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(ImageLocation));
        }

        var spacing = parameter switch
        {
            double d => d,
            string s when double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) =>
                parsed,
            _ => Spacing,
        };

        return imageLocation switch
        {
            ImageLocation.Left => new Thickness(0, 0, spacing, 0),
            ImageLocation.Top => new Thickness(0, 0, 0, spacing),
            ImageLocation.Right => new Thickness(spacing, 0, 0, 0),
            ImageLocation.Bottom => new Thickness(0, spacing, 0, 0),
            ImageLocation.Center => new Thickness(0, 0, 0, 0),
            _ => throw new SwitchCaseDefaultException(imageLocation),
        };
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}