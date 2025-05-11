namespace Atc.Wpf.Controls.BaseControls.Internal.ValueConverters;

/// <summary>
/// ValueConverter: ImageLocation to Thickness (Margin).
/// </summary>
[ValueConversion(typeof(ImageLocation), typeof(Thickness))]
internal sealed class ImageLocationToMarginMultiValueConverter : IMultiValueConverter
{
    public static readonly ImageLocationToMarginMultiValueConverter Instance = new();

    public double SpacingToContent { get; set; } = 5.0;

    public double SpacingToBorder { get; set; } = 1.0;

    public object Convert(
        object[] values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (values.Length < 1 || values[0] is not ImageLocation imageLocation)
        {
            return new Thickness(0);
        }

        var spacingToContent = SpacingToContent;
        if (values.Length > 1)
        {
            spacingToContent = values[1] switch
            {
                double d => d,
                string s when double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) =>
                    parsed,
                _ => SpacingToContent,
            };
        }

        var spacingToBorder = SpacingToBorder;
        if (values.Length > 2)
        {
            spacingToBorder = values[2] switch
            {
                double d => d,
                string s when double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) =>
                    parsed,
                _ => SpacingToBorder,
            };
        }

        return imageLocation switch
        {
            ImageLocation.Left => new Thickness(spacingToBorder, 0, spacingToContent, 0),
            ImageLocation.Top => new Thickness(0, spacingToBorder, 0, spacingToContent),
            ImageLocation.Right => new Thickness(spacingToContent, 0, spacingToBorder, 0),
            ImageLocation.Bottom => new Thickness(0, spacingToContent, 0, spacingToBorder),
            ImageLocation.Center => new Thickness(0, 0, 0, 0),
            _ => throw new SwitchCaseDefaultException(imageLocation),
        };
    }

    public object[] ConvertBack(
        object? value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}