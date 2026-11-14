namespace Atc.Wpf.Controls.Clocks.Internal;

/// <summary>
/// Shared helpers for the clock controls.
/// </summary>
internal static class ClockHelper
{
    /// <summary>
    /// Wraps a time into a time of day (0:00:00 up to, but not including, 24:00:00).
    /// </summary>
    public static TimeSpan ToTimeOfDay(TimeSpan time)
    {
        var ticks = time.Ticks % TimeSpan.TicksPerDay;
        if (ticks < 0)
        {
            ticks += TimeSpan.TicksPerDay;
        }

        return TimeSpan.FromTicks(ticks);
    }

    /// <summary>
    /// Gets the hex text (#AARRGGBB) of a solid colour brush, or <see langword="null"/> for any other brush.
    /// </summary>
    public static string? ToHex(Brush? brush)
        => brush is SolidColorBrush solidColorBrush
            ? solidColorBrush.Color.ToString(CultureInfo.InvariantCulture)
            : null;

    /// <summary>
    /// Sets a brush property from a hex colour. An empty value restores the theme brush
    /// (or clears the property when there is no theme brush).
    /// </summary>
    public static void SetBrushFromHex(
        FrameworkElement element,
        DependencyProperty property,
        string? hexValue,
        string? themeResourceKey)
    {
        if (string.IsNullOrWhiteSpace(hexValue))
        {
            if (themeResourceKey is null)
            {
                element.ClearValue(property);
            }
            else
            {
                element.SetResourceReference(property, themeResourceKey);
            }

            return;
        }

        element.SetValue(property, SolidColorBrushHelper.GetBrushFromHex(hexValue));
    }
}