namespace Atc.Wpf.Components.Printing;

/// <summary>
/// Resolves the pixels-per-DIP value used to lay out text on printed pages.
/// </summary>
internal static class PrintDpiHelper
{
    /// <summary>
    /// Gets the pixels-per-DIP of <paramref name="visual"/>; without a visual, that of the
    /// application's main window; and when there is neither (tray-only, during startup, or hosted
    /// in another UI framework), 1.0 (96 DPI).
    /// </summary>
    /// <param name="visual">The visual being printed, or <see langword="null"/>.</param>
    /// <returns>The pixels-per-DIP value to pass to <see cref="FormattedText"/>.</returns>
    public static double GetPixelsPerDip(Visual? visual)
    {
        var dpiSource = visual ?? Application.Current?.MainWindow;
        return dpiSource is null
            ? 1.0
            : VisualTreeHelper.GetDpi(dpiSource).PixelsPerDip;
    }
}