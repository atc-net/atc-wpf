// ReSharper disable InvertIf
namespace Atc.Wpf.Helpers;

/// <summary>
/// A Helper class for the SolidColorBrush.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
public static class SolidColorBrushHelper
{
    private static readonly ConcurrentDictionary<string, SolidColorBrush> BaseBrushes = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<int, Dictionary<SolidColorBrush, string>> BrushNames = new();

    public static SolidColorBrush? GetBrushFromString(
        string value)
        => GetBrushFromString(value, CultureInfo.CurrentUICulture);

    public static SolidColorBrush? GetBrushFromString(
        string value,
        CultureInfo culture)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        ArgumentNullException.ThrowIfNull(culture);

        EnsureBrushNamesForCulture(culture);

        var brushKey = GetBrushKeyFromCulture(culture);

        if (value.StartsWith('#') &&
            ColorConverter.ConvertFromString(value) is Color color)
        {
            return BrushNames[brushKey]
                .FirstOrDefault(x => string.Equals(x.Key.Color.ToString(GlobalizationConstants.EnglishCultureInfo), color.ToString(GlobalizationConstants.EnglishCultureInfo), StringComparison.OrdinalIgnoreCase))
                .Key;
        }

        if (!value.Contains(' ', StringComparison.Ordinal))
        {
            EnsureBaseBrushes();
            if (BaseBrushes.TryGetValue(value, out var baseBrush))
            {
                return baseBrush;
            }
        }

        return BrushNames[brushKey]
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

    public static IList<string> GetBaseBrushNames()
    {
        EnsureBaseBrushes();
        return BaseBrushes.Select(x => x.Key).ToList();
    }

    public static IList<string> GetBasicBrushNames()
        => ColorHelper.GetBasicColorNames();

    public static string? GetBaseBrushNameFromBrush(
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

        foreach (var entry in resourceSet.OfType<DictionaryEntry>())
        {
            try
            {
                if (ColorConverter.ConvertFromString(entry.Key.ToString()) is Color color)
                {
                    var brush = new SolidColorBrush(color);
                    brush.Freeze();
                    dictionary.Add(brush, entry.Value!.ToString()!);
                }
            }
            catch (Exception)
            {
                Trace.TraceError($"{entry.Key} is not a valid color key!");
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