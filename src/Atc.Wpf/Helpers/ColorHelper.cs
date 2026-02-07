// ReSharper disable InvertIf
// ReSharper disable InconsistentNaming
// ReSharper disable MoveLocalFunctionAfterJumpStatement
namespace Atc.Wpf.Helpers;

/// <summary>
/// A Helper class for the Color-Struct.
/// </summary>
/// <remarks>
/// <para>
/// This class uses internal caches that are bounded by design:
/// </para>
/// <list type="bullet">
/// <item><description><c>BaseColors</c> - populated once from WPF's <see cref="Colors"/> class (~140 colors)</description></item>
/// <item><description><c>ColorNames</c> - populated per-culture from resource files</description></item>
/// </list>
/// <para>
/// Hex color lookups are NOT cached - they use <see cref="ColorConverter"/> directly.
/// </para>
/// </remarks>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
public static class ColorHelper
{
    /// <summary>
    /// Cache of color keys to Color values. Bounded by WPF's Colors class (~140 colors).
    /// Lazily initialized once from WPF's <see cref="Colors"/> class via reflection.
    /// </summary>
    private static readonly Lazy<Dictionary<string, Color>> LazyBaseColors = new(
        static () =>
        {
            var colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
            var dict = new Dictionary<string, Color>(colorProperties.Length, StringComparer.Ordinal);
            foreach (var p in colorProperties.OrderBy(static p => p.Name, StringComparer.Ordinal))
            {
                dict.TryAdd(p.Name, (Color)p.GetValue(obj: null, index: null)!);
            }

            return dict;
        },
        LazyThreadSafetyMode.ExecutionAndPublication);

    private static Dictionary<string, Color> BaseColors => LazyBaseColors.Value;

    /// <summary>
    /// Cache of culture LCID to color name dictionaries. Bounded by cultures used.
    /// </summary>
    private static readonly ConcurrentDictionary<int, Dictionary<Color, string>> ColorNames = new();

    /// <summary>
    /// Reverse lookup cache: color name (case-insensitive) to Color. Built alongside <see cref="ColorNames"/>.
    /// </summary>
    private static readonly ConcurrentDictionary<int, Dictionary<string, Color>> ColorNameToColor = new();

    public static void InitializeWithSupportedLanguages()
    {
        EnsureColorNamesForCulture(new CultureInfo(GlobalizationLcidConstants.UnitedStates));
        EnsureColorNamesForCulture(new CultureInfo(GlobalizationLcidConstants.GreatBritain));
        EnsureColorNamesForCulture(new CultureInfo(GlobalizationLcidConstants.Denmark));
        EnsureColorNamesForCulture(new CultureInfo(GlobalizationLcidConstants.Germany));
    }

    public static Color[] GetColors()
        => BaseColors
            .Select(x => x.Value)
            .ToArray();

    public static Color[] GetBasicColors()
    {
        var colors = new List<Color>();
        foreach (var key in GetBasicColorKeys())
        {
            var colorFromKey = GetColorFromName(
                key,
                GlobalizationConstants.EnglishCultureInfo);
            if (colorFromKey is { } color)
            {
                colors.Add(color);
            }
        }

        return [.. colors];
    }

    public static Color? GetColorFromString(string value)
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

        if (!value.Contains(' ', StringComparison.Ordinal) &&
            BaseColors.TryGetValue(value, out var baseColor))
        {
            return baseColor;
        }

        EnsureColorNamesForCulture(culture);

        var cultureKey = GetColorKeyFromCulture(culture);
        if (ColorNameToColor.TryGetValue(cultureKey, out var nameToColor) &&
            nameToColor.TryGetValue(value, out var namedColor))
        {
            return namedColor;
        }

        return default(Color);
    }

    public static Color? GetColorFromName(string colorName)
        => GetColorFromString(colorName, CultureInfo.CurrentUICulture);

    public static Color? GetColorFromName(
        string colorName,
        CultureInfo culture)
    {
        ArgumentException.ThrowIfNullOrEmpty(colorName);
        ArgumentNullException.ThrowIfNull(culture);

        if (colorName.StartsWith('#'))
        {
            throw new ArgumentException(
                "It is a hex value",
                nameof(colorName));
        }

        return GetColorFromString(
            colorName,
            culture);
    }

    public static Color? GetColorFromHex(string hexValue)
    {
        ArgumentException.ThrowIfNullOrEmpty(hexValue);

        if (!hexValue.StartsWith('#'))
        {
            throw new ArgumentException(
                "It is not a hex value",
                nameof(hexValue));
        }

        if (hexValue.Length is not (9 or 7 or 4))
        {
            throw new ArgumentException(
                "Invalid format",
                nameof(hexValue));
        }

        return GetColorFromString(
            hexValue,
            CultureInfo.InvariantCulture);
    }

    public static IList<string> GetAllColorNames()
        => GetAllColorNames(CultureInfo.CurrentUICulture);

    public static IList<string> GetAllColorNames(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        EnsureColorNamesForCulture(culture);

        var colorNames = ColorNames[GetColorKeyFromCulture(culture)];
        var values = new List<string>(colorNames.Values);
        values.Sort(StringComparer.Ordinal);
        return values;
    }

    public static IList<string> GetColorKeys()
        => BaseColors
            .Select(x => x.Key)
            .ToList();

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
            .OrderBy(
                x => x,
                StringComparer.Ordinal)
            .ToList();
    }

    public static string? GetColorKeyFromColor(Color brush)
        => BaseColors
            .FirstOrDefault(x => string.Equals(
                x.Value.ToString(GlobalizationConstants.EnglishCultureInfo),
                brush.ToString(GlobalizationConstants.EnglishCultureInfo),
                StringComparison.Ordinal))
            .Key;

    public static string? GetColorNameFromColor(Color color)
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
            .FirstOrDefault(x => string.Equals(
                x.Key.ToString(GlobalizationConstants.EnglishCultureInfo),
                color.ToString(GlobalizationConstants.EnglishCultureInfo),
                StringComparison.OrdinalIgnoreCase))
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

    public static string? GetColorKeyFromHex(string hexValue)
    {
        ArgumentException.ThrowIfNullOrEmpty(hexValue);

        if (!hexValue.StartsWith('#') &&
            !hexValue.StartsWith(
                "0x",
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "It is not a hex value",
                nameof(hexValue));
        }

        if (hexValue.StartsWith('#') &&
            hexValue.Length == 7)
        {
            hexValue = string.Concat("#FF", hexValue.AsSpan(1));
        }
        else if (hexValue.StartsWith(
                     "0x",
                     StringComparison.Ordinal) &&
                 hexValue.Length == 8)
        {
            hexValue = string.Concat("#FF", hexValue.AsSpan(2));
        }
        else if (hexValue.StartsWith(
                     "0x",
                     StringComparison.Ordinal))
        {
            hexValue = string.Concat("#", hexValue.AsSpan(2));
        }

        if (hexValue == "#FF00FFFF")
        {
            return "Cyan";
        }

        return BaseColors
            .FirstOrDefault(x => string.Equals(
                x.Value.ToString(GlobalizationConstants.EnglishCultureInfo),
                hexValue,
                StringComparison.Ordinal))
            .Key;
    }

    public static string? GetColorNameFromHex(string hexValue)
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
        var item = BaseColors
            .FirstOrDefault(x => string.Equals(
                x.Key,
                colorKey,
                StringComparison.Ordinal));

        return string.IsNullOrEmpty(item.Key)
            ? null
            : GetColorNameFromColor(
                item.Value,
                culture);
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
        h = Clamp(
            h,
            0,
            360);
        s = Clamp(
            s,
            0,
            1);
        v = Clamp(
            v,
            0,
            1);

        byte f(double n)
        {
            var k = (n + (h / 60)) % 6;
            var value = v - (v * s * System.Math.Max(
                System.Math.Min(
                    System.Math.Min(
                        k,
                        4 - k),
                    1),
                0));
            return (byte)System.Math.Round(value * 255);
        }

        return Color.FromRgb(
            f(5),
            f(3),
            f(1));
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

    private static void EnsureColorNamesForCulture(CultureInfo culture)
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
                    dictionary.TryAdd(
                        color,
                        entry.Value!.ToString()!);
                }
            }
            catch
            {
                // Ignored
            }
        }

        ColorNames.TryAdd(
            colorKey,
            dictionary);

        var reverseDictionary = new Dictionary<string, Color>(dictionary.Count, StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in dictionary)
        {
            reverseDictionary.TryAdd(kvp.Value, kvp.Key);
        }

        ColorNameToColor.TryAdd(colorKey, reverseDictionary);
    }

    private static int GetColorKeyFromCulture(CultureInfo culture)
        => culture.LCID == CultureInfo.InvariantCulture.LCID
            ? GlobalizationConstants.EnglishCultureInfo.LCID
            : culture.LCID;
}