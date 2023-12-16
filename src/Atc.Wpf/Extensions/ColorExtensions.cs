namespace Atc.Wpf.Extensions;

public static class ColorExtensions
{
    public static string? GetBaseBrushName(
        this Color color)
        => ColorHelper.GetBaseColorNameFromColor(color);

    public static string? GetColorName(
        this Color color)
        => ColorHelper.GetColorNameFromColor(color);

    public static string? GetColorName(
        this Color color,
        CultureInfo culture,
        bool includeColorHex = false,
        bool useAlphaChannel = true)
        => ColorHelper.GetColorNameFromColor(
            color,
            culture,
            includeColorHex,
            useAlphaChannel);

    /// <summary>
    /// Linearly interpolates between two colors.
    /// </summary>
    /// <param name="colorFrom">Source Color.</param>
    /// <param name="colorTo">Destination Color.</param>
    /// <param name="amount">A value between 0 and 1.0 indicating the weight of colorTo.</param>
    /// <remarks>This function linearly interpolates each component of a Color separately and returns a Color with the new component values.</remarks>
    /// <returns>The interpolated Color.</returns>
    public static Color Lerp(
        this Color colorFrom,
        Color colorTo,
        float amount)
    {
        // Start colors as lerp-able floats
        float sr = colorFrom.R, sg = colorFrom.G, sb = colorFrom.B;

        // End colors as lerp-able floats
        float er = colorTo.R, eg = colorTo.G, eb = colorTo.B;

        // Lerp the colors to get the difference
        byte r = (byte)Lerp(sr, er, amount),
            g = (byte)Lerp(sg, eg, amount),
            b = (byte)Lerp(sb, eb, amount);

        // Return the new color
        return Color.FromRgb(r, g, b);
    }

    /// <summary>
    /// Lerps the specified start.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <param name="amount">The amount.</param>
    private static float Lerp(
        float start,
        float end,
        float amount)
    {
        var difference = end - start;
        var adjusted = difference * amount;
        return start + adjusted;
    }
}