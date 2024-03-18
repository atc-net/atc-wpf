namespace Atc.Wpf.Theming.Helpers;

public static class ThemeManagerHelper
{
    private static readonly ConcurrentDictionary<string, Color> CacheColors = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, SolidColorBrush> CacheBrushes = new(StringComparer.Ordinal);

    static ThemeManagerHelper()
    {
        ThemeManager.Current.ThemeChanged += OnThemeChanged;
    }

    public static Color GetColorByResourceKey(
        AtcAppsColorKeyType colorKey)
        => GetColorByResourceKey(colorKey.ToString());

    public static Color GetColorByResourceKey(
        string resourceKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(resourceKey);

        if (!resourceKey.StartsWith("AtcApps.Colors.", StringComparison.OrdinalIgnoreCase))
        {
            resourceKey = $"AtcApps.Colors.{resourceKey}";
        }

        var currentTheme = ThemeManager.Current.DetectTheme(Application.Current)!;
        var cacheKey = $"{currentTheme.BaseColorScheme}_{resourceKey}";

        if (CacheColors.TryGetValue(cacheKey, out var cacheColor))
        {
            return cacheColor;
        }

        var color = (Color)currentTheme.Resources[resourceKey]!;
        CacheColors.TryAdd(cacheKey, color);
        return color;
    }

    public static SolidColorBrush GetBrushByResourceKey(
        AtcAppsBrushKeyType brushKey)
        => GetBrushByResourceKey(brushKey.ToString());

    public static SolidColorBrush GetBrushByResourceKey(
        string resourceKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(resourceKey);

        if (!resourceKey.StartsWith("AtcApps.Brushes.", StringComparison.OrdinalIgnoreCase))
        {
            resourceKey = $"AtcApps.Brushes.{resourceKey}";
        }

        var currentTheme = ThemeManager.Current.DetectTheme(Application.Current)!;
        var cacheKey = $"{currentTheme.BaseColorScheme}_{resourceKey}";

        if (CacheBrushes.TryGetValue(cacheKey, out var cacheBrush))
        {
            return cacheBrush;
        }

        var brush = (SolidColorBrush)currentTheme.Resources[resourceKey]!;
        if (brush.CanFreeze)
        {
            brush.Freeze();
        }

        CacheBrushes.TryAdd(cacheKey, brush);
        return brush;
    }

    public static Color GetPrimaryAccentColor()
    {
        var currentTheme = ThemeManager.Current.DetectTheme(Application.Current)!;
        return currentTheme.PrimaryAccentColor;
    }

    public static SolidColorBrush GetPrimaryAccentBrush()
    {
        var color = GetPrimaryAccentColor();
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    public static void SetThemeAndAccent(
        Application current,
        string themeAndAccent)
    {
        var theme = string.IsNullOrEmpty(themeAndAccent)
            ? "Light.Blue"
            : themeAndAccent;

        ThemeManager.Current.ChangeTheme(current, theme);
    }

    private static void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        CacheColors.Clear();
        CacheBrushes.Clear();
    }
}