// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Monitoring;

/// <summary>
/// Renders a glyph for the Pause toolbar toggle: shows the Play triangle when
/// paused (so the button reads "press to resume") and the Pause bars when
/// running (so the button reads "press to pause").
/// </summary>
[ValueConversion(typeof(bool), typeof(string))]
internal sealed class PausedToGlyphValueConverter : IValueConverter
{
    private const string PauseGlyph = "⏸"; // ⏸
    private const string PlayGlyph = "⏵";  // ⏵

    public static readonly PausedToGlyphValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is true ? PlayGlyph : PauseGlyph;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a one-way converter.");
}
