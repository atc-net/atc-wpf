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

    public static void InitializeWithSupportedLanguages()
    {
        EnsureBaseBrushes();
        EnsureBrushNamesForCulture(new CultureInfo(GlobalizationLcidConstants.UnitedStates));
        EnsureBrushNamesForCulture(new CultureInfo(GlobalizationLcidConstants.GreatBritain));
        EnsureBrushNamesForCulture(new CultureInfo(GlobalizationLcidConstants.Denmark));
        EnsureBrushNamesForCulture(new CultureInfo(GlobalizationLcidConstants.Germany));
    }

    public static SolidColorBrush[] GetBrushes()
    {
        EnsureBaseBrushes();

        return BaseBrushes
            .Select(x => x.Value)
            .ToArray();
    }

    public static SolidColorBrush[] GetBasicBrushes()
    {
        EnsureBaseBrushes();

        var brushes = new List<SolidColorBrush>();
        foreach (var key in GetBasicBrushKeys())
        {
            var brushFromKey = GetBrushFromName(key, GlobalizationConstants.EnglishCultureInfo);
            if (brushFromKey is not null)
            {
                brushes.Add(brushFromKey);
            }
        }

        return [.. brushes];
    }

    public static SolidColorBrush? GetBrushFromString(
        string value)
        => GetBrushFromString(value, CultureInfo.CurrentUICulture);

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

        if (!value.Contains(' ', StringComparison.Ordinal))
        {
            EnsureBaseBrushes();

            if (BaseBrushes.TryGetValue(value, out var baseBrush))
            {
                return baseBrush;
            }
        }

        EnsureBrushNamesForCulture(culture);

        return BrushNames[GetBrushKeyFromCulture(culture)]
            .FirstOrDefault(x => string.Equals(x.Value, value, StringComparison.OrdinalIgnoreCase))
            .Key;
    }

    public static SolidColorBrush? GetBrushFromName(
        string brushName)
        => GetBrushFromString(brushName, CultureInfo.CurrentUICulture);

    public static SolidColorBrush? GetBrushFromName(
        string brushName,
        CultureInfo culture)
    {
        ArgumentException.ThrowIfNullOrEmpty(brushName);
        ArgumentNullException.ThrowIfNull(culture);

        if (brushName.StartsWith('#'))
        {
            throw new ArgumentException("It is a hex value", nameof(brushName));
        }

        return GetBrushFromString(brushName, culture);
    }

    public static SolidColorBrush? GetBrushFromHex(
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

        return GetBrushFromString(hexValue, CultureInfo.InvariantCulture);
    }

    public static IList<string> GetAllBrushNames()
        => GetAllBrushNames(CultureInfo.CurrentUICulture);

    public static IList<string> GetAllBrushNames(
        CultureInfo culture)
        => ColorHelper.GetAllColorNames(culture);

    public static IList<string> GetBrushKeys()
    {
        EnsureBaseBrushes();

        return BaseBrushes
            .Select(x => x.Key)
            .ToList();
    }

    public static IList<string> GetBasicBrushKeys()
        => ColorHelper.GetBasicColorKeys();

    public static string? GetBrushKeyFromBrush(
        SolidColorBrush brush)
    {
        EnsureBaseBrushes();

        return BaseBrushes
            .FirstOrDefault(x => string.Equals(x.Value.Color.ToString(GlobalizationConstants.EnglishCultureInfo), brush.Color.ToString(GlobalizationConstants.EnglishCultureInfo), StringComparison.Ordinal))
            .Key;
    }

    public static string? GetBrushNameFromBrush(
        SolidColorBrush brush)
        => GetBrushNameFromBrush(brush, CultureInfo.CurrentUICulture);

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
            .FirstOrDefault(x => string.Equals(x.Key.Color.ToString(GlobalizationConstants.EnglishCultureInfo), brush.Color.ToString(GlobalizationConstants.EnglishCultureInfo), StringComparison.OrdinalIgnoreCase))
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

    public static string? GetBrushKeyFromHex(
        string hexValue)
    {
        ArgumentException.ThrowIfNullOrEmpty(hexValue);

        if (!hexValue.StartsWith('#') &&
            !hexValue.StartsWith("0x", StringComparison.Ordinal))
        {
            throw new ArgumentException("It is not a hex value", nameof(hexValue));
        }

        EnsureBaseBrushes();

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

        return BaseBrushes
            .FirstOrDefault(x => string.Equals(x.Value.ToString(GlobalizationConstants.EnglishCultureInfo), hexValue, StringComparison.Ordinal))
            .Key;
    }

    public static string? GetBrushNameFromHex(
        string hexValue)
        => GetBrushNameFromHex(hexValue, CultureInfo.CurrentUICulture);

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

    public static string? GetBrushNameFromKey(
        string brushKey,
        CultureInfo culture)
    {
        EnsureBaseBrushes();

        if ("Aqua".Equals(brushKey, StringComparison.Ordinal))
        {
            brushKey = "Cyan";
        }
        else if ("Fuchsia".Equals(brushKey, StringComparison.Ordinal))
        {
            brushKey = "Magenta";
        }

        var item = BaseBrushes
            .FirstOrDefault(x => string.Equals(x.Key, brushKey, StringComparison.Ordinal));

        return string.IsNullOrEmpty(item.Key)
            ? null
            : GetBrushNameFromBrush(item.Value, culture);
    }

    private static void EnsureBaseBrushes()
    {
        if (!BaseBrushes.IsEmpty)
        {
            return;
        }

        var colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);

        var colorDictionary = colorProperties
            .ToDictionary(p => p.Name, p => (Color)p.GetValue(obj: null, index: null)!, StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x.Key, StringComparer.Ordinal);

        foreach (var item in colorDictionary)
        {
            var brush = new SolidColorBrush(item.Value);
            brush.Freeze();

            BaseBrushes.TryAdd(item.Key, brush);
        }
    }

    private static void EnsureBrushNamesForCulture(
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var brushKey = GetBrushKeyFromCulture(culture);
        if (BrushNames.ContainsKey(brushKey))
        {
            return;
        }

        var dictionary = new Dictionary<SolidColorBrush, string>();

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
                "Aqua".Equals(entryKey, StringComparison.Ordinal) ||
                "Fuchsia".Equals(entryKey, StringComparison.Ordinal))
            {
                continue;
            }

            try
            {
                if (ColorConverter.ConvertFromString(entryKey) is Color color)
                {
                    var brush = new SolidColorBrush(color);
                    brush.Freeze();

                    dictionary.TryAdd(brush, entry.Value!.ToString()!);
                }
            }
            catch (FormatException)
            {
                // Ignored
            }
        }

        BrushNames.TryAdd(brushKey, dictionary);
    }

    private static int GetBrushKeyFromCulture(
        CultureInfo culture)
        => culture.LCID == CultureInfo.InvariantCulture.LCID
            ? GlobalizationConstants.EnglishCultureInfo.LCID
            : culture.LCID;
}