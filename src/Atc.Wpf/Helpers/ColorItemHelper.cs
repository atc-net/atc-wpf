// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Helpers;

public static class ColorItemHelper
{
    private static readonly ConcurrentDictionary<int, ColorItem[]> CacheColorItems = new();
    private static readonly ConcurrentDictionary<int, ColorItem[]> CacheBasicColorItems = new();

    public static ColorItem[] GetColorItems()
    {
        if (CacheColorItems.TryGetValue(
                CultureInfo.CurrentUICulture.LCID,
                out var cacheItems))
        {
            return cacheItems;
        }

        var coloKeys = ColorHelper.GetColorKeys();
        var items = GetColorItemsByKeys(coloKeys);

        CacheColorItems.TryAdd(
            CultureInfo.CurrentUICulture.LCID,
            items);

        return items;
    }

    public static ColorItem[] GetBasicColorItems()
    {
        if (CacheBasicColorItems.TryGetValue(
                CultureInfo.CurrentUICulture.LCID,
                out var cacheItems))
        {
            return cacheItems;
        }

        var coloKeys = ColorHelper.GetBasicColorKeys();
        var items = GetColorItemsByKeys(coloKeys);

        CacheBasicColorItems.TryAdd(
            CultureInfo.CurrentUICulture.LCID,
            items);

        return items;
    }

    private static ColorItem[] GetColorItemsByKeys(IEnumerable<string> coloKeys)
    {
        var list = new List<ColorItem>();
        foreach (var itemKey in coloKeys)
        {
            var translatedName = ColorHelper.GetColorNameFromKey(
                itemKey,
                CultureInfo.CurrentUICulture);

            var color = ColorHelper.GetColorFromName(
                itemKey,
                GlobalizationConstants.EnglishCultureInfo);
            if (color is null)
            {
                continue;
            }

            var showcaseBrush = SolidColorBrushHelper.GetBrushFromName(
                itemKey,
                GlobalizationConstants.EnglishCultureInfo);
            if (showcaseBrush is null)
            {
                continue;
            }

            list.Add(
                new ColorItem(
                    Key: itemKey,
                    DisplayName: translatedName ?? "#" + itemKey,
                    DisplayHexCode: color.Value
                        .ToString(GlobalizationConstants.EnglishCultureInfo),
                    BorderColorBrush: showcaseBrush,
                    ColorBrush: showcaseBrush));
        }

        return [.. list.OrderBy(
            x => x.DisplayName,
            StringComparer.Ordinal)];
    }
}