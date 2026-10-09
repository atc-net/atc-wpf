namespace Atc.Wpf.Theming.Helpers;

/// <summary>
/// Helpers for reading colors and brushes from the current theme and for changing the theme.
/// Looked-up resources are cached per theme until the theme changes.
/// </summary>
public static class ThemeManagerHelper
{
    private static readonly ConcurrentDictionary<string, Color> CacheColors = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, SolidColorBrush> CacheBrushes = new(StringComparer.Ordinal);

    static ThemeManagerHelper()
    {
        ThemeManager.Current.ThemeChanged += OnThemeChanged;
    }

    /// <summary>
    /// Gets the color for the specified theme color key from the current theme.
    /// </summary>
    /// <param name="colorKey">The theme color key.</param>
    /// <returns>The color, or <see cref="Colors.DeepPink"/> if the resource is not found.</returns>
    public static Color GetColorByResourceKey(AtcAppsColorKeyType colorKey)
        => GetColorByResourceKey(colorKey.ToString());

    /// <summary>
    /// Gets the color for the specified resource key from the current theme;
    /// the <c>AtcApps.Colors.</c> prefix is added when missing.
    /// </summary>
    /// <param name="resourceKey">The resource key, with or without the <c>AtcApps.Colors.</c> prefix.</param>
    /// <returns>The color, or <see cref="Colors.DeepPink"/> if the resource is not found.</returns>
    public static Color GetColorByResourceKey(string resourceKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(resourceKey);

        if (!resourceKey.StartsWith("AtcApps.Colors.", StringComparison.OrdinalIgnoreCase))
        {
            resourceKey = $"AtcApps.Colors.{resourceKey}";
        }

        if (resourceKey.Contains(".Bootstrap", StringComparison.Ordinal))
        {
            resourceKey = resourceKey
                .Replace(
                    ".Bootstrap",
                    ".Bootstrap.",
                    StringComparison.Ordinal)
                .Replace(
                    "..",
                    ".",
                    StringComparison.Ordinal);
        }

        var currentTheme = ThemeManager.Current.DetectTheme(Application.Current)!;
        var cacheKey = $"{currentTheme.BaseColorScheme}_{resourceKey}";

        if (CacheColors.TryGetValue(
            cacheKey,
            out var cacheColor))
        {
            return cacheColor;
        }

        object? resource = null;
        if (resourceKey.Contains(".Bootstrap", StringComparison.Ordinal))
        {
            resource = Application.Current.Resources[resourceKey];
        }

        resource ??= currentTheme.Resources[resourceKey] ?? Application.Current.Resources[resourceKey];

        if (resource is null)
        {
            return Colors.DeepPink;
        }

        var color = (Color)resource;
        CacheColors.TryAdd(
            cacheKey,
            color);
        return color;
    }

    /// <summary>
    /// Gets the brush for the specified theme brush key from the current theme.
    /// </summary>
    /// <param name="brushKey">The theme brush key.</param>
    /// <returns>The frozen brush, or <see cref="Brushes.DeepPink"/> if the resource is not found.</returns>
    public static SolidColorBrush GetBrushByResourceKey(
        AtcAppsBrushKeyType brushKey)
        => GetBrushByResourceKey(brushKey.ToString());

    /// <summary>
    /// Gets the brush for the specified resource key from the current theme;
    /// the <c>AtcApps.Brushes.</c> prefix is added when missing.
    /// </summary>
    /// <param name="resourceKey">The resource key, with or without the <c>AtcApps.Brushes.</c> prefix.</param>
    /// <returns>The frozen brush, or <see cref="Brushes.DeepPink"/> if the resource is not found.</returns>
    public static SolidColorBrush GetBrushByResourceKey(string resourceKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(resourceKey);

        if (!resourceKey.StartsWith("AtcApps.Brushes.", StringComparison.OrdinalIgnoreCase))
        {
            resourceKey = $"AtcApps.Brushes.{resourceKey}";
        }

        if (resourceKey.Contains(".Bootstrap", StringComparison.Ordinal))
        {
            resourceKey = resourceKey
                .Replace(
                    ".Bootstrap",
                    ".Bootstrap.",
                    StringComparison.Ordinal)
                .Replace(
                    "..",
                    ".",
                    StringComparison.Ordinal);
        }

        var currentTheme = ThemeManager.Current.DetectTheme(Application.Current)!;
        var cacheKey = $"{currentTheme.BaseColorScheme}_{resourceKey}";

        if (CacheBrushes.TryGetValue(
            cacheKey,
            out var cacheBrush))
        {
            return cacheBrush;
        }

        object? resource = null;
        if (resourceKey.Contains(".Bootstrap", StringComparison.Ordinal))
        {
            resource = Application.Current.Resources[resourceKey];
        }

        resource ??= currentTheme.Resources[resourceKey] ?? Application.Current.Resources[resourceKey];

        if (resource is null)
        {
            return Brushes.DeepPink;
        }

        var brush = (SolidColorBrush)resource;
        if (brush.CanFreeze)
        {
            brush.Freeze();
        }

        CacheBrushes.TryAdd(
            cacheKey,
            brush);
        return brush;
    }

    /// <summary>
    /// Gets the primary accent color of the current application theme.
    /// </summary>
    /// <returns>The primary accent color.</returns>
    public static Color GetPrimaryAccentColor()
        => ThemeManager.Current.DetectTheme(Application.Current)!.PrimaryAccentColor;

    /// <summary>
    /// Gets a frozen brush of the primary accent color of the current application theme.
    /// </summary>
    /// <returns>A new frozen <see cref="SolidColorBrush"/>.</returns>
    public static SolidColorBrush GetPrimaryAccentBrush()
    {
        var color = GetPrimaryAccentColor();
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    /// <summary>
    /// Changes the theme of the application.
    /// </summary>
    /// <param name="current">The application whose theme is changed.</param>
    /// <param name="themeAndAccent">The theme name, such as <c>Dark.Blue</c>; defaults to <c>Light.Blue</c> when empty.</param>
    public static void SetThemeAndAccent(
        Application current,
        string themeAndAccent)
    {
        var theme = string.IsNullOrEmpty(themeAndAccent)
            ? "Light.Blue"
            : themeAndAccent;

        ThemeManager.Current.ChangeTheme(
            current,
            theme);
    }

    private static void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        CacheColors.Clear();
        CacheBrushes.Clear();
    }
}