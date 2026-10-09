// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Helpers;

/// <summary>
/// A Helper class for the SolidColorBrush.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
public static class SolidColorBrushHelper
{
    private static readonly ConcurrentDictionary<string, SolidColorBrush> BaseBrushes = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<int, Dictionary<SolidColorBrush, string>> BrushNames = new();
    private static readonly ConcurrentDictionary<int, Dictionary<string, SolidColorBrush>> BrushNamesReverse = new();

    /// <summary>Preloads the known brushes and the localized brush names for the supported languages (en-US, en-GB, da-DK and de-DE).</summary>
    public static void InitializeWithSupportedLanguages()
    {
        EnsureBaseBrushes();
        EnsureBrushNamesForCulture(new CultureInfo(GlobalizationLcidConstants.UnitedStates));
        EnsureBrushNamesForCulture(new CultureInfo(GlobalizationLcidConstants.GreatBritain));
        EnsureBrushNamesForCulture(new CultureInfo(GlobalizationLcidConstants.Denmark));
        EnsureBrushNamesForCulture(new CultureInfo(GlobalizationLcidConstants.Germany));
    }

    /// <summary>Gets all known brushes.</summary>
    public static SolidColorBrush[] GetBrushes()
    {
        EnsureBaseBrushes();

        return BaseBrushes
            .Select(x => x.Value)
            .ToArray();
    }

    /// <summary>Gets the basic brushes.</summary>
    public static SolidColorBrush[] GetBasicBrushes()
    {
        EnsureBaseBrushes();

        var brushes = new List<SolidColorBrush>();
        foreach (var key in GetBasicBrushKeys())
        {
            var brushFromKey = GetBrushFromName(
                key,
                GlobalizationConstants.EnglishCultureInfo);
            if (brushFromKey is not null)
            {
                brushes.Add(brushFromKey);
            }
        }

        return [.. brushes];
    }

    /// <summary>Gets a brush from a hex value or a color name in the current UI culture.</summary>
    public static SolidColorBrush? GetBrushFromString(string value)
        => GetBrushFromString(value, CultureInfo.CurrentUICulture);

    /// <summary>Gets a brush from a hex value or a color name in the specified culture.</summary>
    public static SolidColorBrush? GetBrushFromString(
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
                    return new SolidColorBrush(color);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        if (!value.Contains(
                ' ',
                StringComparison.Ordinal))
        {
            EnsureBaseBrushes();

            if (BaseBrushes.TryGetValue(
                    value,
                    out var baseBrush))
            {
                return baseBrush;
            }
        }

        EnsureBrushNamesForCulture(culture);

        if (!BrushNamesReverse.TryGetValue(GetBrushKeyFromCulture(culture), out var nameToBrush))
        {
            return default;
        }

        return nameToBrush.TryGetValue(value, out var brush)
            ? brush
            : null;
    }

    /// <summary>Gets a brush from a color name in the current UI culture.</summary>
    public static SolidColorBrush? GetBrushFromName(string brushName)
        => GetBrushFromString(brushName, CultureInfo.CurrentUICulture);

    /// <summary>Gets a brush from a color name in the specified culture.</summary>
    public static SolidColorBrush? GetBrushFromName(
        string brushName,
        CultureInfo culture)
    {
        ArgumentException.ThrowIfNullOrEmpty(brushName);
        ArgumentNullException.ThrowIfNull(culture);

        if (brushName.StartsWith('#'))
        {
            throw new ArgumentException(
                "It is a hex value",
                nameof(brushName));
        }

        return GetBrushFromString(
            brushName,
            culture);
    }

    /// <summary>Gets a brush from a hex value in the format #RGB, #RRGGBB or #AARRGGBB.</summary>
    public static SolidColorBrush? GetBrushFromHex(string hexValue)
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

        return GetBrushFromString(
            hexValue,
            CultureInfo.InvariantCulture);
    }

    /// <summary>Gets all localized brush names for the current UI culture, sorted.</summary>
    public static IList<string> GetAllBrushNames()
        => GetAllBrushNames(CultureInfo.CurrentUICulture);

    /// <summary>Gets all localized brush names for the specified culture, sorted.</summary>
    public static IList<string> GetAllBrushNames(CultureInfo culture)
        => ColorHelper.GetAllColorNames(culture);

    /// <summary>Gets the keys of all known brushes.</summary>
    public static IList<string> GetBrushKeys()
    {
        EnsureBaseBrushes();

        return BaseBrushes
            .Select(x => x.Key)
            .ToList();
    }

    /// <summary>Gets the keys of the basic brushes, sorted.</summary>
    public static IList<string> GetBasicBrushKeys()
        => ColorHelper.GetBasicColorKeys();

    /// <summary>Gets the key of the known brush whose color matches the brush, or <see langword="null"/> if there is none.</summary>
    public static string? GetBrushKeyFromBrush(SolidColorBrush brush)
    {
        EnsureBaseBrushes();

        return BaseBrushes
            .FirstOrDefault(x => string.Equals(
                x.Value.Color.ToString(GlobalizationConstants.EnglishCultureInfo),
                brush.Color.ToString(GlobalizationConstants.EnglishCultureInfo),
                StringComparison.Ordinal))
            .Key;
    }

    /// <summary>Gets the localized name of the brush in the current UI culture.</summary>
    public static string? GetBrushNameFromBrush(SolidColorBrush brush)
        => GetBrushNameFromBrush(brush, CultureInfo.CurrentUICulture);

    /// <summary>Gets the localized name of the brush in the specified culture, optionally followed by its hex value.</summary>
    public static string? GetBrushNameFromBrush(
        SolidColorBrush brush,
        CultureInfo culture,
        bool includeColorHex = false,
        bool useAlphaChannel = true)
    {
        ArgumentNullException.ThrowIfNull(brush);
        ArgumentNullException.ThrowIfNull(culture);

        EnsureBrushNamesForCulture(culture);

        var brushName = BrushNames[GetBrushKeyFromCulture(culture)]
            .FirstOrDefault(x => string.Equals(
                x.Key.Color.ToString(GlobalizationConstants.EnglishCultureInfo),
                brush.Color.ToString(GlobalizationConstants.EnglishCultureInfo),
                StringComparison.OrdinalIgnoreCase))
            .Value;

        if (!includeColorHex)
        {
            return brushName;
        }

        var colorHex = useAlphaChannel
            ? brush.Color.ToString(GlobalizationConstants.EnglishCultureInfo)
            : $"#{brush.Color.R:X2}{brush.Color.G:X2}{brush.Color.B:X2}";

        return $"{brushName} ({colorHex})";
    }

    /// <summary>Gets the key of the known brush that matches a hex value starting with # or 0x, or <see langword="null"/> if there is none.</summary>
    public static string? GetBrushKeyFromHex(string hexValue)
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

        EnsureBaseBrushes();

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

        return BaseBrushes
            .FirstOrDefault(x => string.Equals(
                x.Value.ToString(GlobalizationConstants.EnglishCultureInfo),
                hexValue,
                StringComparison.Ordinal))
            .Key;
    }

    /// <summary>Gets the localized name of the brush for a hex value in the current UI culture.</summary>
    public static string? GetBrushNameFromHex(string hexValue)
        => GetBrushNameFromHex(hexValue, CultureInfo.CurrentUICulture);

    /// <summary>Gets the localized name of the brush for a hex value in the specified culture, optionally followed by its hex value.</summary>
    public static string? GetBrushNameFromHex(
        string hexValue,
        CultureInfo culture,
        bool includeColorHex = false,
        bool useAlphaChannel = true)
    {
        var brush = GetBrushFromHex(hexValue);
        return brush is null
            ? null
            : GetBrushNameFromBrush(
                brush,
                culture,
                includeColorHex,
                useAlphaChannel);
    }

    /// <summary>Gets the localized name of the brush with the given key in the specified culture.</summary>
    public static string? GetBrushNameFromKey(
        string brushKey,
        CultureInfo culture)
    {
        EnsureBaseBrushes();

        if ("Aqua".Equals(
                brushKey,
                StringComparison.Ordinal))
        {
            brushKey = "Cyan";
        }
        else if ("Fuchsia".Equals(
                     brushKey,
                     StringComparison.Ordinal))
        {
            brushKey = "Magenta";
        }

        var item = BaseBrushes
            .FirstOrDefault(x => string.Equals(
                x.Key,
                brushKey,
                StringComparison.Ordinal));

        return string.IsNullOrEmpty(item.Key)
            ? null
            : GetBrushNameFromBrush(
                item.Value,
                culture);
    }

    private static void EnsureBaseBrushes()
    {
        if (!BaseBrushes.IsEmpty)
        {
            return;
        }

        var colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);

        var colorDictionary = colorProperties
            .ToDictionary(
                p => p.Name,
                p => (Color)p.GetValue(
                    obj: null,
                    index: null)!,
                StringComparer.OrdinalIgnoreCase)
            .OrderBy(
                x => x.Key,
                StringComparer.Ordinal);

        foreach (var item in colorDictionary)
        {
            var brush = new SolidColorBrush(item.Value);
            brush.Freeze();

            BaseBrushes.TryAdd(
                item.Key,
                brush);
        }
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - building two parallel lookups in one pass keeps the resource enumeration cost down.")]
    private static void EnsureBrushNamesForCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var brushKey = GetBrushKeyFromCulture(culture);
        if (BrushNames.ContainsKey(brushKey))
        {
            return;
        }

        var dictionary = new Dictionary<SolidColorBrush, string>();
        var reverse = new Dictionary<string, SolidColorBrush>(StringComparer.OrdinalIgnoreCase);

        var rm = new ResourceManager(typeof(ColorNames));
        var resourceSet = rm.GetResourceSet(
            culture,
            createIfNotExists: true,
            tryParents: true);

        if (resourceSet is null)
        {
            return;
        }

        EnsureBaseBrushes();

        foreach (var entry in resourceSet.OfType<DictionaryEntry>())
        {
            var entryKey = entry.Key.ToString()!;
            if (string.IsNullOrEmpty(BaseBrushes.FirstOrDefault(x => x.Key == entryKey).Key) ||
                "Aqua".Equals(
                    entryKey,
                    StringComparison.Ordinal) ||
                "Fuchsia".Equals(
                    entryKey,
                    StringComparison.Ordinal))
            {
                continue;
            }

            try
            {
                if (ColorConverter.ConvertFromString(entryKey) is Color color)
                {
                    var brush = new SolidColorBrush(color);
                    brush.Freeze();

                    var localizedName = entry.Value!.ToString()!;
                    dictionary.TryAdd(
                        brush,
                        localizedName);
                    reverse.TryAdd(
                        localizedName,
                        brush);
                }
            }
            catch (FormatException)
            {
                // Ignored
            }
        }

        BrushNames.TryAdd(
            brushKey,
            dictionary);
        BrushNamesReverse.TryAdd(
            brushKey,
            reverse);
    }

    private static int GetBrushKeyFromCulture(CultureInfo culture)
        => culture.LCID == CultureInfo.InvariantCulture.LCID
            ? GlobalizationConstants.EnglishCultureInfo.LCID
            : culture.LCID;
}