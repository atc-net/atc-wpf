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
    /// <remarks>
    /// A missing key is written once to <see cref="System.Diagnostics.Trace"/>; use
    /// <see cref="TryGetColorByResourceKey(string, out Color)"/> to tell a missing key from a real color.
    /// </remarks>
    public static Color GetColorByResourceKey(string resourceKey)
    {
        if (TryGetColorByResourceKey(resourceKey, out var color))
        {
            return color;
        }

        TraceOnce.Warn(
            $"theme-color:{resourceKey}",
            $"ThemeManagerHelper: no theme color '{resourceKey}' (is a theme applied?); using DeepPink.");
        return Colors.DeepPink;
    }

    /// <summary>
    /// Gets the color for the specified resource key from the current theme;
    /// the <c>AtcApps.Colors.</c> prefix is added when missing.
    /// </summary>
    /// <param name="resourceKey">The resource key, with or without the <c>AtcApps.Colors.</c> prefix.</param>
    /// <param name="color">The color when found; otherwise <see langword="default"/>.</param>
    /// <returns><see langword="true"/> when the current theme has the color.</returns>
    public static bool TryGetColorByResourceKey(
        string resourceKey,
        out Color color)
    {
        ArgumentException.ThrowIfNullOrEmpty(resourceKey);
        color = default;

        resourceKey = NormalizeResourceKey(resourceKey, "AtcApps.Colors.");

        if (Application.Current is not { } application ||
            ThemeManager.Current.DetectTheme(application) is not { } currentTheme)
        {
            return false;
        }

        var cacheKey = $"{currentTheme.BaseColorScheme}_{resourceKey}";

        if (CacheColors.TryGetValue(
            cacheKey,
            out var cacheColor))
        {
            color = cacheColor;
            return true;
        }

        object? resource = null;
        if (resourceKey.Contains(".Bootstrap", StringComparison.Ordinal))
        {
            resource = application.Resources[resourceKey];
        }

        resource ??= currentTheme.Resources[resourceKey] ?? application.Resources[resourceKey];

        if (resource is not Color found)
        {
            return false;
        }

        CacheColors.TryAdd(
            cacheKey,
            found);
        color = found;
        return true;
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
    /// <remarks>
    /// A missing key is written once to <see cref="System.Diagnostics.Trace"/>; use
    /// <see cref="TryGetBrushByResourceKey(string, out SolidColorBrush?)"/> to tell a missing key from a real brush.
    /// </remarks>
    public static SolidColorBrush GetBrushByResourceKey(string resourceKey)
    {
        if (TryGetBrushByResourceKey(resourceKey, out var brush))
        {
            return brush!;
        }

        TraceOnce.Warn(
            $"theme-brush:{resourceKey}",
            $"ThemeManagerHelper: no theme brush '{resourceKey}' (is a theme applied?); using DeepPink.");
        return Brushes.DeepPink;
    }

    /// <summary>
    /// Gets the brush for the specified resource key from the current theme;
    /// the <c>AtcApps.Brushes.</c> prefix is added when missing.
    /// </summary>
    /// <param name="resourceKey">The resource key, with or without the <c>AtcApps.Brushes.</c> prefix.</param>
    /// <param name="brush">The frozen brush when found; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the current theme has the brush.</returns>
    public static bool TryGetBrushByResourceKey(
        string resourceKey,
        out SolidColorBrush? brush)
    {
        ArgumentException.ThrowIfNullOrEmpty(resourceKey);
        brush = null;

        resourceKey = NormalizeResourceKey(resourceKey, "AtcApps.Brushes.");

        if (Application.Current is not { } application ||
            ThemeManager.Current.DetectTheme(application) is not { } currentTheme)
        {
            return false;
        }

        var cacheKey = $"{currentTheme.BaseColorScheme}_{resourceKey}";

        if (CacheBrushes.TryGetValue(
            cacheKey,
            out var cacheBrush))
        {
            brush = cacheBrush;
            return true;
        }

        object? resource = null;
        if (resourceKey.Contains(".Bootstrap", StringComparison.Ordinal))
        {
            resource = application.Resources[resourceKey];
        }

        resource ??= currentTheme.Resources[resourceKey] ?? application.Resources[resourceKey];

        if (resource is not SolidColorBrush found)
        {
            return false;
        }

        if (found.CanFreeze)
        {
            found.Freeze();
        }

        CacheBrushes.TryAdd(
            cacheKey,
            found);
        brush = found;
        return true;
    }

    /// <summary>
    /// Gets the primary accent color of the current application theme.
    /// </summary>
    /// <returns>The primary accent color, or <see cref="Colors.DeepPink"/> if no theme is detected.</returns>
    public static Color GetPrimaryAccentColor()
    {
        var application = Application.Current;
        var currentTheme = application is null
            ? null
            : ThemeManager.Current.DetectTheme(application);

        return currentTheme?.PrimaryAccentColor ?? Colors.DeepPink;
    }

    /// <summary>
    /// Gets a frozen brush of the primary accent color of the current application theme.
    /// </summary>
    /// <returns>A new frozen <see cref="SolidColorBrush"/>, of <see cref="Colors.DeepPink"/> if no theme is detected.</returns>
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

    // Adds the prefix when missing and spells the Bootstrap keys as the theme defines them (".Bootstrap.").
    private static string NormalizeResourceKey(
        string resourceKey,
        string prefix)
    {
        if (!resourceKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            resourceKey = prefix + resourceKey;
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

        return resourceKey;
    }
}