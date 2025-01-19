// ReSharper disable InvertIf
// ReSharper disable InconsistentNaming
// ReSharper disable MoveLocalFunctionAfterJumpStatement
namespace Atc.Wpf.Helpers;

/// <summary>
/// A Helper class for the Color-Struct.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
public static class ColorHelper
{
    private static readonly ConcurrentDictionary<string, Color> BaseColors = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<int, Dictionary<Color, string>> ColorNames = new();

    public static void InitializeWithSupportedLanguages()
    {
        EnsureBaseColors();
        EnsureColorNamesForCulture(new CultureInfo(GlobalizationLcidConstants.UnitedStates));
        EnsureColorNamesForCulture(new CultureInfo(GlobalizationLcidConstants.GreatBritain));
        EnsureColorNamesForCulture(new CultureInfo(GlobalizationLcidConstants.Denmark));
        EnsureColorNamesForCulture(new CultureInfo(GlobalizationLcidConstants.Germany));
    }

    public static Color[] GetColors()
    {
        EnsureBaseColors();

        return BaseColors
            .Select(x => x.Value)
            .ToArray();
    }

    public static Color[] GetBasicColors()
    {
        EnsureBaseColors();

        var colors = new List<Color>();
        foreach (var key in GetBasicColorKeys())
        {
            var colorFromKey = GetColorFromName(key, GlobalizationConstants.EnglishCultureInfo);
            if (colorFromKey is { } color)
            {
                colors.Add(color);
            }
        }

        return [.. colors];
    }

    public static Color? GetColorFromString(
        string value)
        => GetColorFromString(value, CultureInfo.CurrentUICulture);

    public static Color? GetColorFromString(
        string value,
        CultureInfo culture)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        ArgumentNullException.ThrowIfNull(culture);

        if (value.StartsWith('#'))
        {
            try
            {
                if (ColorConverter.ConvertFromString(value) is Color color)
                {
                    return color;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        if (!value.Contains(' ', StringComparison.Ordinal))
        {
            EnsureBaseColors();

            if (BaseColors.TryGetValue(value, out var baseColor))
            {
                return baseColor;
            }
        }

        EnsureColorNamesForCulture(culture);

        return ColorNames[GetColorKeyFromCulture(culture)]
            .FirstOrDefault(x => string.Equals(x.Value, value, StringComparison.OrdinalIgnoreCase))
            .Key;
    }

    public static Color? GetColorFromName(
        string colorName)
        => GetColorFromString(colorName, CultureInfo.CurrentUICulture);

    public static Color? GetColorFromName(
        string colorName,
        CultureInfo culture)
    {
        ArgumentException.ThrowIfNullOrEmpty(colorName);
        ArgumentNullException.ThrowIfNull(culture);

        if (colorName.StartsWith('#'))
        {
            throw new ArgumentException("It is a hex value", nameof(colorName));
        }

        return GetColorFromString(colorName, culture);
    }

    public static Color? GetColorFromHex(
        string hexValue)
    {
        ArgumentException.ThrowIfNullOrEmpty(hexValue);

        if (!hexValue.StartsWith('#'))
        {
            throw new ArgumentException("It is not a hex value", nameof(hexValue));
        }

        if (hexValue.Length is not (9 or 7 or 4))
        {
            throw new ArgumentException("Invalid format", nameof(hexValue));
        }

        return GetColorFromString(hexValue, CultureInfo.InvariantCulture);
    }

    public static IList<string> GetAllColorNames()
        => GetAllColorNames(CultureInfo.CurrentUICulture);

    public static IList<string> GetAllColorNames(
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        EnsureColorNamesForCulture(culture);

        var colorNames = ColorNames[GetColorKeyFromCulture(culture)];
        return colorNames
            .Select(x => x.Value)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
    }

    public static IList<string> GetColorKeys()
    {
        EnsureBaseColors();

        return BaseColors
            .Select(x => x.Key)
            .ToList();
    }

    public static IList<string> GetBasicColorKeys()
    {
        var list = new List<string>
        {
            "White",
            "Silver",
            "Gray",
            "Black",
            "Red",
            "Maroon",
            "Yellow",
            "Olive",
            "Lime",
            "Green",
            "Aqua",
            "Teal",
            "Blue",
            "Navy",
            "Fuchsia",
            "Purple",
        };

        return list
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
    }

    public static string? GetColorKeyFromColor(
        Color brush)
    {
        EnsureBaseColors();

        return BaseColors
            .FirstOrDefault(x => string.Equals(x.Value.ToString(GlobalizationConstants.EnglishCultureInfo), brush.ToString(GlobalizationConstants.EnglishCultureInfo), StringComparison.Ordinal))
            .Key;
    }

    public static string? GetColorNameFromColor(
        Color color)
        => GetColorNameFromColor(color, CultureInfo.CurrentUICulture);

    public static string? GetColorNameFromColor(
        Color color,
        CultureInfo culture,
        bool includeColorHex = false,
        bool useAlphaChannel = true)
    {
        ArgumentNullException.ThrowIfNull(culture);

        EnsureColorNamesForCulture(culture);

        var colorName = ColorNames[GetColorKeyFromCulture(culture)]
            .FirstOrDefault(x => string.Equals(x.Key.ToString(GlobalizationConstants.EnglishCultureInfo), color.ToString(GlobalizationConstants.EnglishCultureInfo), StringComparison.OrdinalIgnoreCase))
            .Value;

        if (!includeColorHex)
        {
            return colorName;
        }

        var colorHex = useAlphaChannel
            ? color.ToString(GlobalizationConstants.EnglishCultureInfo)
            : $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        return $"{colorName} ({colorHex})";
    }

    public static string? GetColorKeyFromHex(
        string hexValue)
    {
        ArgumentException.ThrowIfNullOrEmpty(hexValue);

        if (!hexValue.StartsWith('#') &&
            !hexValue.StartsWith("0x", StringComparison.Ordinal))
        {
            throw new ArgumentException("It is not a hex value", nameof(hexValue));
        }

        EnsureBaseColors();

        if (hexValue.StartsWith('#') &&
            hexValue.Length == 7)
        {
            hexValue = hexValue.Replace("#", "#FF", StringComparison.Ordinal);
        }
        else if (hexValue.StartsWith("0x", StringComparison.Ordinal) &&
                 hexValue.Length == 8)
        {
            hexValue = hexValue.Replace("0x", "0xFF", StringComparison.Ordinal);
        }

        if (hexValue.StartsWith("0x", StringComparison.Ordinal))
        {
            hexValue = hexValue.Replace("0x", "#", StringComparison.Ordinal);
        }

        if (hexValue == "#FF00FFFF")
        {
            return "Cyan";
        }

        return BaseColors
            .FirstOrDefault(x => string.Equals(x.Value.ToString(GlobalizationConstants.EnglishCultureInfo), hexValue, StringComparison.Ordinal))
            .Key;
    }

    public static string? GetColorNameFromHex(
        string hexValue)
        => GetColorNameFromHex(hexValue, CultureInfo.CurrentUICulture);

    public static string? GetColorNameFromHex(
        string hexValue,
        CultureInfo culture,
        bool includeColorHex = false,
        bool useAlphaChannel = true)
    {
        var color = GetColorFromHex(hexValue);
        return color is null
            ? null
            : GetColorNameFromColor(
                color.Value,
                culture,
                includeColorHex,
                useAlphaChannel);
    }

    public static string? GetColorNameFromKey(
        string colorKey,
        CultureInfo culture)
    {
        EnsureBaseColors();

        var item = BaseColors
            .FirstOrDefault(x => string.Equals(x.Key, colorKey, StringComparison.Ordinal));

        return string.IsNullOrEmpty(item.Key)
            ? null
            : GetColorNameFromColor(item.Value, culture);
    }

    /// <summary>
    /// Get a color from HSV / HSB values.
    /// </summary>
    /// <param name="h">Hue, [0 - 360]</param>
    /// <param name="s">Saturation, [0, 1]</param>
    /// <param name="v">Value, [0, 1]</param>
    /// <returns>The color created from hsv values.</returns>
    /// <remarks>Algorithm from https://en.wikipedia.org/wiki/HSL_and_HSV #From Hsv</remarks>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "OK.")]
    public static Color GetColorFromHsv(
        double h,
        double s,
        double v)
    {
        h = Clamp(h, 0, 360);
        s = Clamp(s, 0, 1);
        v = Clamp(v, 0, 1);

        byte f(double n)
        {
            var k = (n + (h / 60)) % 6;
            var value = v - (v * s * System.Math.Max(System.Math.Min(System.Math.Min(k, 4 - k), 1), 0));
            return (byte)System.Math.Round(value * 255);
        }

        return Color.FromRgb(f(5), f(3), f(1));
    }

    private static double Clamp(
        double value,
        double min,
        double max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max
            ? max
            : value;
    }

    private static void EnsureBaseColors()
    {
        if (!BaseColors.IsEmpty)
        {
            return;
        }

        var colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);

        var colorDictionary = colorProperties
            .ToDictionary(p => p.Name, p => (Color)p.GetValue(obj: null, index: null)!, StringComparer.Ordinal)
            .OrderBy(x => x.Key, StringComparer.Ordinal);

        foreach (var item in colorDictionary)
        {
            BaseColors.TryAdd(item.Key, item.Value);
        }
    }

    private static void EnsureColorNamesForCulture(
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var colorKey = GetColorKeyFromCulture(culture);
        if (ColorNames.ContainsKey(colorKey))
        {
            return;
        }

        var dictionary = new Dictionary<Color, string>();

        var rm = new ResourceManager(typeof(ColorNames));
        var resourceSet = rm.GetResourceSet(
            culture,
            createIfNotExists: true,
            tryParents: true);

        if (resourceSet is null)
        {
            return;
        }

        EnsureBaseColors();

        foreach (var entry in resourceSet.OfType<DictionaryEntry>())
        {
            var entryKey = entry.Key.ToString()!;
            if (string.IsNullOrEmpty(BaseColors.FirstOrDefault(x => x.Key == entryKey).Key))
            {
                continue;
            }

            try
            {
                if (ColorConverter.ConvertFromString(entryKey) is Color color)
                {
                    dictionary.TryAdd(color, entry.Value!.ToString()!);
                }
            }
            catch
            {
                // Ignored
            }
        }

        ColorNames.TryAdd(colorKey, dictionary);
    }

    private static int GetColorKeyFromCulture(
        CultureInfo culture)
        => culture.LCID == CultureInfo.InvariantCulture.LCID
            ? GlobalizationConstants.EnglishCultureInfo.LCID
            : culture.LCID;
}