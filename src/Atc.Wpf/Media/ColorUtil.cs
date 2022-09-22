namespace Atc.Wpf.Media;

/// <summary>
/// Color Util.
/// </summary>
public static class ColorUtil
{
    /// <summary>
    /// Known Colors.
    /// </summary>
    [SuppressMessage("Minor Code Smell", "S2386:Mutable fields should not be \"public static\"", Justification = "OK.")]
    [SuppressMessage("Minor Bug", "S3887:Mutable, non-private fields should not be \"readonly\"", Justification = "OK.")]
    public static readonly IDictionary<string, Color> KnownColors = GetKnownColors();

    /// <summary>
    /// Gets the solid color brush from hex.
    /// </summary>
    /// <param name="hexaColor">Color of the hexa.</param>
    /// <returns>The solidColorBrush from hexa color.</returns>
    public static SolidColorBrush GetSolidColorBrushFromHex(string hexaColor)
    {
        return new SolidColorBrush(GetColorFromHex(hexaColor));
    }

    /// <summary>
    /// Gets the color from hex.
    /// </summary>
    /// <param name="hexaColor">Color of the hexa.</param>
    /// <returns>The color from hexa color.</returns>
    public static Color GetColorFromHex(string hexaColor)
    {
        ArgumentNullException.ThrowIfNull(hexaColor);

        if (!hexaColor.StartsWith('#'))
        {
            hexaColor = "#" + hexaColor;
        }

        switch (hexaColor.Length)
        {
            case 4:
                var r = hexaColor.Substring(1, 1);
                var g = hexaColor.Substring(2, 1);
                var b = hexaColor.Substring(3, 1);
                return Color.FromArgb(
                    Convert.ToByte("FF", 16),
                    Convert.ToByte($"{r}{r}", 16),
                    Convert.ToByte($"{g}{g}", 16),
                    Convert.ToByte($"{b}{b}", 16));
            case 7:
                hexaColor = hexaColor.Replace("#", "#FF", StringComparison.Ordinal);
                break;
        }

        if (hexaColor.Length != 9)
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentException("Invalid format: " + hexaColor, nameof(hexaColor));
        }

        return Color.FromArgb(
            Convert.ToByte(hexaColor.Substring(1, 2), 16),
            Convert.ToByte(hexaColor.Substring(3, 2), 16),
            Convert.ToByte(hexaColor.Substring(5, 2), 16),
            Convert.ToByte(hexaColor.Substring(7, 2), 16));
    }

    /// <summary>
    /// Formats the color string.
    /// </summary>
    /// <param name="stringToFormat">
    /// The string to format.
    /// </param>
    /// <param name="isUsingAlphaChannel">
    /// if set to <c>true</c> [is using alpha channel].
    /// </param>
    /// <returns>
    /// The <see cref="string" />.
    /// </returns>
    public static string FormatColorString(string stringToFormat, bool isUsingAlphaChannel)
    {
        ArgumentNullException.ThrowIfNull(stringToFormat);

        if (!isUsingAlphaChannel && (stringToFormat.Length == 9))
        {
            return stringToFormat.Remove(1, 2);
        }

        return stringToFormat;
    }

    /// <summary>
    /// Converts an RGB color to an HSV color.
    /// </summary>
    /// <param name="r">
    /// The r.
    /// </param>
    /// <param name="b">
    /// The b.
    /// </param>
    /// <param name="g">
    /// The g.
    /// </param>
    /// <returns>
    /// The <see cref="HsvColor" />.
    /// </returns>
    public static HsvColor ConvertRgbToHsv(int r, int b, int g)
    {
        double h = 0;
        double s;

        double min = System.Math.Min(System.Math.Min(r, g), b);
        double v = System.Math.Max(System.Math.Max(r, g), b);
        var delta = v - min;

        if (v.IsEqual(0))
        {
            s = 0;
        }
        else
        {
            s = delta / v;
        }

        if (s.IsEqual(0))
        {
            h = 0.0;
        }
        else
        {
            var iv = (int)v;
            if (r == iv)
            {
                h = (g - b) / delta;
            }
            else if (g == iv)
            {
                h = 2 + ((b - r) / delta);
            }
            else if (b == iv)
            {
                h = 4 + ((r - g) / delta);
            }

            h *= 60;
            if (h < 0.0)
            {
                h += 360;
            }
        }

        return new HsvColor
        {
            H = h,
            S = s,
            V = v / 255,
        };
    }

    /// <summary>
    /// Converts an HSV color to an RGB color.
    /// </summary>
    /// <param name="h">
    /// The h.
    /// </param>
    /// <param name="s">
    /// The s.
    /// </param>
    /// <param name="v">
    /// The v.
    /// </param>
    /// <returns>
    /// The <see cref="Color" />.
    /// </returns>
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    public static Color ConvertHsvToRgb(double h, double s, double v)
    {
        double r;
        double g;
        double b;

        if (s.IsEqual(0))
        {
            r = v;
            g = v;
            b = v;
        }
        else
        {
            if (h.IsEqual(360))
            {
                h = 0;
            }
            else
            {
                h /= 60;
            }

            var i = (int)System.Math.Truncate(h);
            var f = h - i;
            var p = v * (1.0 - s);
            var q = v * (1.0 - (s * f));
            var t = v * (1.0 - (s * (1.0 - f)));

            switch (i)
            {
                case 0:
                {
                    r = v;
                    g = t;
                    b = p;
                    break;
                }

                case 1:
                {
                    r = q;
                    g = v;
                    b = p;
                    break;
                }

                case 2:
                {
                    r = p;
                    g = v;
                    b = t;
                    break;
                }

                case 3:
                {
                    r = p;
                    g = q;
                    b = v;
                    break;
                }

                case 4:
                {
                    r = t;
                    g = p;
                    b = v;
                    break;
                }

                default:
                {
                    r = v;
                    g = p;
                    b = q;
                    break;
                }
            }
        }

        return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }

    /// <summary>
    /// Generates a list of colors with hues ranging from 0 360 and a saturation and value of 1.
    /// </summary>
    /// <returns>
    /// The list of colors.
    /// </returns>
    public static IList<Color> GenerateHsvSpectrum()
    {
        var colorsList = new List<Color>(8);

        for (var i = 0; i < 29; i++)
        {
            colorsList.Add(ConvertHsvToRgb(i * 12, 1, 1));
        }

        colorsList.Add(ConvertHsvToRgb(0, 1, 1));

        return colorsList;
    }

    /// <summary>
    /// Linearly interpolates between two colors.
    /// </summary>
    /// <param name="colorFrom">Source Color.</param>
    /// <param name="colorTo">Destination Color.</param>
    /// <param name="amount">A value between 0 and 1.0 indicating the weight of colorTo.</param>
    /// <remarks>This function linearly interpolates each component of a Color separately and returns a Color with the new component values.</remarks>
    /// <returns>The interpolated Color.</returns>
    public static Color Lerp(Color colorFrom, Color colorTo, float amount)
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
    private static float Lerp(float start, float end, float amount)
    {
        var difference = end - start;
        var adjusted = difference * amount;
        return start + adjusted;
    }

    /// <summary>
    /// Gets the known colors.
    /// </summary>
    /// <returns>
    /// The dictionary of key and colors.
    /// </returns>
    private static Dictionary<string, Color> GetKnownColors()
    {
        var colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
        return colorProperties.ToDictionary(p => p.Name, p => (Color)p.GetValue(obj: null, index: null)!, StringComparer.OrdinalIgnoreCase);
    }
}