namespace Atc.Wpf.Sample.SamplesWpfFontIcons;

internal sealed class FontIconViewerImageConverter : IMultiValueConverter
{
    public static readonly FontIconViewerImageConverter Instance = new();

    private static readonly FontIconDrawingImageValueConverter FontConverter = new();

    public object? Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (values.Length < 2 ||
            values[0] is not Enum enumValue ||
            values[1] is not Color color)
        {
            return null;
        }

        var brush = new SolidColorBrush(color);
        brush.Freeze();

        var drawingImage = FontConverter.Convert(
            enumValue,
            targetType: null,
            brush,
            culture: null) as DrawingImage;

        drawingImage?.Freeze();

        return drawingImage;
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}