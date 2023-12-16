namespace Atc.Wpf.Media;

/// <summary>
/// HsvColor Util.
/// </summary>
public static class HsvColorUtil
{
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
}