namespace Atc.Wpf.Theming.Theming;

/// <summary>
/// Turns the colors of a base color scheme (Light or Dark) into high-contrast colors from the Windows system palette.
/// </summary>
/// <remarks>
/// The accent becomes the system highlight color. Every other base color becomes either the window (background)
/// color or the window text color, depending on whether it is closer to the scheme's own background or foreground,
/// so the same rule works for both Light and Dark. Colors whose key mentions "Disabled" become the gray text color.
/// Transparency is kept, so overlays stay overlays.
/// </remarks>
internal static class HighContrastColorScheme
{
    private const string ColorKeyPrefix = "AtcApps.Colors.";
    private const string BackgroundKey = "AtcApps.Colors.ThemeBackground";
    private const string ForegroundKey = "AtcApps.Colors.ThemeForeground";

    private static readonly string[] AccentKeys =
    [
        "AtcApps.Colors.AccentBase",
        "AtcApps.Colors.Accent",
        "AtcApps.Colors.Accent2",
        "AtcApps.Colors.Accent3",
        "AtcApps.Colors.Accent4",
        "AtcApps.Colors.Highlight",
    ];

    /// <summary>
    /// Sets the high-contrast colors in <paramref name="values"/>, overriding the base color scheme values.
    /// </summary>
    /// <param name="values">The color scheme values; they take precedence over the base color scheme values.</param>
    /// <param name="baseColorSchemeValues">The values of the base color scheme (Light or Dark).</param>
    /// <param name="palette">The high-contrast system colors.</param>
    public static void Fill(
        IDictionary<string, string> values,
        IReadOnlyDictionary<string, string> baseColorSchemeValues,
        HighContrastPalette palette)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(baseColorSchemeValues);
        ArgumentNullException.ThrowIfNull(palette);

        foreach (var key in AccentKeys)
        {
            values[key] = Format(palette.Highlight);
        }

        values["AtcApps.Colors.IdealForeground"] = Format(palette.HighlightText);

        if (!TryGetColor(baseColorSchemeValues, BackgroundKey, out var background) ||
            !TryGetColor(baseColorSchemeValues, ForegroundKey, out var foreground))
        {
            return;
        }

        var backgroundLuminance = Luminance(background);
        var foregroundLuminance = Luminance(foreground);

        foreach (var (key, value) in baseColorSchemeValues)
        {
            if (!key.StartsWith(ColorKeyPrefix, StringComparison.Ordinal) ||
                !TryParse(value, out var color))
            {
                continue;
            }

            Color target;
            if (key.Contains("Disabled", StringComparison.Ordinal))
            {
                target = palette.GrayText;
            }
            else
            {
                var luminance = Luminance(color);
                target = System.Math.Abs(luminance - backgroundLuminance) <= System.Math.Abs(luminance - foregroundLuminance)
                    ? palette.Window
                    : palette.WindowText;
            }

            values[key] = Format(Color.FromArgb(color.A, target.R, target.G, target.B));
        }
    }

    /// <summary>
    /// Gives border and line brushes that ended up in the window color the window text color, so they stay visible.
    /// </summary>
    /// <remarks>
    /// The color rule can't tell a border shade from a background shade, because the same gray is used for both;
    /// the brush key can.
    /// </remarks>
    /// <param name="resources">The generated theme resources.</param>
    /// <param name="palette">The high-contrast system colors.</param>
    public static void MakeBordersVisible(
        ResourceDictionary resources,
        HighContrastPalette palette)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(palette);

        var invisibleBorderKeys = resources.Keys
            .OfType<string>()
            .Where(IsBorderKey)
            .Where(key => resources[key] is SolidColorBrush brush && IsSameColor(brush.Color, palette.Window))
            .ToList();

        foreach (var key in invisibleBorderKeys)
        {
            var brush = new SolidColorBrush(palette.WindowText);
            brush.Freeze();
            resources[key] = brush;
        }
    }

    private static bool IsBorderKey(string key)
        => key.Contains("Border", StringComparison.Ordinal) ||
           key.Contains("Stroke", StringComparison.Ordinal) ||
           key.Contains("Separator", StringComparison.Ordinal);

    private static bool IsSameColor(
        Color color,
        Color other)
        => color.R == other.R && color.G == other.G && color.B == other.B;

    private static bool TryGetColor(
        IReadOnlyDictionary<string, string> values,
        string key,
        out Color color)
    {
        color = default;
        return values.TryGetValue(key, out var value) && TryParse(value, out color);
    }

    private static bool TryParse(
        string value,
        out Color color)
    {
        color = default;
        if (!value.StartsWith('#'))
        {
            return false;
        }

        try
        {
            color = (Color)ColorConverter.ConvertFromString(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    // Relative luminance with the sRGB coefficients; good enough to tell light from dark.
    private static double Luminance(Color color)
        => ((0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B)) / 255d;

    private static string Format(Color color)
        => color.ToString(GlobalizationConstants.EnglishCultureInfo);
}