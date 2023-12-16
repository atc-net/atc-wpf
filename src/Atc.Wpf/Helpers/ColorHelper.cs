// ReSharper disable InvertIf
namespace Atc.Wpf.Helpers;

/// <summary>
/// A Helper class for the Color-Struct.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
public static class ColorHelper
{
    private static readonly ConcurrentDictionary<string, Color> BaseColors = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<int, Dictionary<Color, string>> ColorNames = new();

    public static Color? GetColorFromString(
        string value)
        => GetColorFromString(value, CultureInfo.CurrentUICulture);

    public static Color? GetColorFromString(
        string value,
        CultureInfo culture)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        ArgumentNullException.ThrowIfNull(culture);

        EnsureColorNamesForCulture(culture);

        var colorKey = GetColorKeyFromCulture(culture);

        if (value.StartsWith('#') &&
            ColorConverter.ConvertFromString(value) is Color color)
        {
            return ColorNames[colorKey]
                .FirstOrDefault(x => string.Equals(x.Key.ToString(GlobalizationConstants.EnglishCultureInfo), color.ToString(GlobalizationConstants.EnglishCultureInfo), StringComparison.OrdinalIgnoreCase))
                .Key;
        }

        if (!value.Contains(' ', StringComparison.Ordinal))
        {
            EnsureBaseColors();
            if (BaseColors.TryGetValue(value, out var baseColor))
            {
                return baseColor;
            }
        }

        return ColorNames[colorKey]
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

    public static IList<string> GetBaseColorNames()
    {
        EnsureBaseColors();
        return BaseColors.Select(x => x.Key).ToList();
    }

    public static IList<string> GetBasicColorNames()
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

    public static string? GetBaseColorNameFromColor(
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
        ArgumentNullException.ThrowIfNull(color);
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

    public static string? GetBaseColorNameFromHex(
        string hexValue)
    {
        EnsureBaseColors();
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

    private static void EnsureBaseColors()
    {
        if (!BaseColors.IsEmpty)
        {
            return;
        }

        var colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);

        var colorDictionary = colorProperties
            .ToDictionary(p => p.Name, p => (Color)p.GetValue(obj: null, index: null)!, StringComparer.OrdinalIgnoreCase)
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

        foreach (var entry in resourceSet.OfType<DictionaryEntry>())
        {
            try
            {
                if (ColorConverter.ConvertFromString(entry.Key.ToString()) is Color color)
                {
                    dictionary.Add(color, entry.Value!.ToString()!);
                }
            }
            catch (Exception)
            {
                Trace.TraceError($"{entry.Key} is not a valid color key!");
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