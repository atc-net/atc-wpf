namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// Adjusts syntax-highlighting colours that were designed for a light background so they stay readable on a dark one.
/// </summary>
internal static class SyntaxColorAdjuster
{
    /// <summary>Colours lighter than this (HSL lightness) are left alone.</summary>
    private const double LightnessThreshold = 0.6;

    /// <summary>Lightness given to a colour that is too dark; keeps the hue so keywords, strings and comments stay distinct.</summary>
    private const double TargetLightness = 0.72;

    /// <summary>Lightness given to near-grey colours (plain text, punctuation), which have no hue worth keeping.</summary>
    private const double TargetGreyLightness = 0.85;

    /// <summary>
    /// Returns a colour that is readable on a dark background: dark colours are lifted to a fixed lightness with
    /// their hue and saturation kept, light colours are returned unchanged.
    /// </summary>
    public static Color EnsureReadableOnDark(Color color)
    {
        var (hue, saturation, lightness) = ToHsl(color);
        if (lightness >= LightnessThreshold)
        {
            return color;
        }

        var targetLightness = saturation < 0.15
            ? TargetGreyLightness
            : TargetLightness;

        var adjusted = FromHsl(hue, saturation, targetLightness);
        adjusted.A = color.A;
        return adjusted;
    }

    private static (double Hue, double Saturation, double Lightness) ToHsl(
        Color color)
    {
        var r = color.R / 255d;
        var g = color.G / 255d;
        var b = color.B / 255d;
        var max = System.Math.Max(r, System.Math.Max(g, b));
        var min = System.Math.Min(r, System.Math.Min(g, b));
        var lightness = (max + min) / 2d;

        if (System.Math.Abs(max - min) < 0.0001)
        {
            return (0, 0, lightness);
        }

        var delta = max - min;
        var saturation = lightness > 0.5
            ? delta / (2d - max - min)
            : delta / (max + min);

        double hue;
        if (System.Math.Abs(max - r) < 0.0001)
        {
            hue = ((g - b) / delta) + (g < b ? 6 : 0);
        }
        else if (System.Math.Abs(max - g) < 0.0001)
        {
            hue = ((b - r) / delta) + 2;
        }
        else
        {
            hue = ((r - g) / delta) + 4;
        }

        return (hue * 60d, saturation, lightness);
    }

    private static Color FromHsl(
        double hue,
        double saturation,
        double lightness)
    {
        if (saturation < 0.0001)
        {
            var grey = ToByte(lightness);
            return Color.FromRgb(grey, grey, grey);
        }

        var q = lightness < 0.5
            ? lightness * (1 + saturation)
            : lightness + saturation - (lightness * saturation);
        var p = (2 * lightness) - q;
        var h = hue / 360d;

        return Color.FromRgb(
            ToByte(HueToChannel(p, q, h + (1d / 3d))),
            ToByte(HueToChannel(p, q, h)),
            ToByte(HueToChannel(p, q, h - (1d / 3d))));
    }

    private static double HueToChannel(
        double p,
        double q,
        double t)
    {
        if (t < 0)
        {
            t += 1;
        }

        if (t > 1)
        {
            t -= 1;
        }

        if (t < 1d / 6d)
        {
            return p + ((q - p) * 6 * t);
        }

        if (t < 1d / 2d)
        {
            return q;
        }

        if (t < 2d / 3d)
        {
            return p + ((q - p) * ((2d / 3d) - t) * 6);
        }

        return p;
    }

    private static byte ToByte(double channel)
        => (byte)System.Math.Clamp(System.Math.Round(channel * 255d), 0, 255);
}