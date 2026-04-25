namespace Atc.Wpf.Forms.FontEditing.Internal;

/// <summary>
/// Multi-binding converter that computes the WCAG 2.x contrast ratio between
/// two SolidColorBrushes (foreground / background) and returns Visibility.Visible
/// when the ratio is below the WCAG AA threshold for normal text (4.5:1).
/// </summary>
internal sealed class ContrastWarningConverter : IMultiValueConverter
{
    public const double WcagAaNormalTextRatio = 4.5;

    public static readonly ContrastWarningConverter Instance = new();

    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (values is null || values.Length < 2)
        {
            return Visibility.Collapsed;
        }

        var foreground = values[0] as SolidColorBrush;
        var background = values[1] as SolidColorBrush;

        if (foreground is null || background is null)
        {
            return Visibility.Collapsed;
        }

        // A fully transparent background means the actual rendering background
        // is whatever is behind us — we can't reason about contrast.
        if (background.Color.A == 0)
        {
            return Visibility.Collapsed;
        }

        var ratio = ComputeContrastRatio(foreground.Color, background.Color);
        return ratio < WcagAaNormalTextRatio ? Visibility.Visible : Visibility.Collapsed;
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
        => throw new NotSupportedException();

    private static double ComputeContrastRatio(
        Color a,
        Color b)
    {
        var luminanceA = RelativeLuminance(a);
        var luminanceB = RelativeLuminance(b);

        var lighter = System.Math.Max(luminanceA, luminanceB);
        var darker = System.Math.Min(luminanceA, luminanceB);

        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double RelativeLuminance(Color color)
    {
        var r = LinearizeChannel(color.R);
        var g = LinearizeChannel(color.G);
        var b = LinearizeChannel(color.B);

        return (0.2126 * r) + (0.7152 * g) + (0.0722 * b);
    }

    private static double LinearizeChannel(byte value)
    {
        var srgb = value / 255.0;
        return srgb <= 0.03928
            ? srgb / 12.92
            : System.Math.Pow((srgb + 0.055) / 1.055, 2.4);
    }
}