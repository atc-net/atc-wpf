namespace Atc.Wpf.Documents.ColorSchemas;

/// <summary>
/// Provides the brushes used for JSON syntax highlighting in light and dark theme modes.
/// </summary>
public static class JsonColorSchema
{
    // Light Theme Colors
    private static readonly SolidColorBrush LightDefaultBrush = CreateFrozenBrush(Colors.Black);
    private static readonly SolidColorBrush LightKeyBrush = CreateFrozenBrush(Color.FromRgb(64, 101, 195));
    private static readonly SolidColorBrush LightStringBrush = CreateFrozenBrush(Color.FromRgb(210, 99, 40));
    private static readonly SolidColorBrush LightNumberBrush = CreateFrozenBrush(Color.FromRgb(76, 175, 80));
    private static readonly SolidColorBrush LightIntegerBrush = CreateFrozenBrush(Color.FromRgb(76, 175, 80));
    private static readonly SolidColorBrush LightFloatBrush = CreateFrozenBrush(Color.FromRgb(76, 175, 80));
    private static readonly SolidColorBrush LightBooleanBrush = CreateFrozenBrush(Color.FromRgb(218, 165, 30));
    private static readonly SolidColorBrush LightGuidBrush = CreateFrozenBrush(Color.FromRgb(156, 39, 176));
    private static readonly SolidColorBrush LightDateBrush = CreateFrozenBrush(Color.FromRgb(121, 134, 203));
    private static readonly SolidColorBrush LightUriBrush = CreateFrozenBrush(Color.FromRgb(77, 140, 183));
    private static readonly SolidColorBrush LightNullBrush = CreateFrozenBrush(Colors.Gray);

    // Dark Theme Colors
    private static readonly SolidColorBrush DarkDefaultBrush = CreateFrozenBrush(Colors.White);
    private static readonly SolidColorBrush DarkKeyBrush = CreateFrozenBrush(Color.FromRgb(79, 195, 247));
    private static readonly SolidColorBrush DarkStringBrush = CreateFrozenBrush(Color.FromRgb(255, 152, 0));
    private static readonly SolidColorBrush DarkNumberBrush = CreateFrozenBrush(Color.FromRgb(139, 195, 74));
    private static readonly SolidColorBrush DarkIntegerBrush = CreateFrozenBrush(Color.FromRgb(139, 195, 74));
    private static readonly SolidColorBrush DarkFloatBrush = CreateFrozenBrush(Color.FromRgb(139, 195, 74));
    private static readonly SolidColorBrush DarkBooleanBrush = CreateFrozenBrush(Color.FromRgb(255, 220, 6));
    private static readonly SolidColorBrush DarkGuidBrush = CreateFrozenBrush(Color.FromRgb(206, 147, 216));
    private static readonly SolidColorBrush DarkDateBrush = CreateFrozenBrush(Color.FromRgb(121, 134, 203));
    private static readonly SolidColorBrush DarkUriBrush = CreateFrozenBrush(Color.FromRgb(86, 156, 203));
    private static readonly SolidColorBrush DarkNullBrush = CreateFrozenBrush(Color.FromRgb(158, 158, 158));

    private static ThemeMode mode = ThemeMode.Light;

    /// <summary>
    /// Gets the default brush for the current theme mode.
    /// </summary>
    public static SolidColorBrush DefaultBrush
        => mode == ThemeMode.Light
            ? LightDefaultBrush
            : DarkDefaultBrush;

    /// <summary>
    /// Gets the brush for property keys for the current theme mode.
    /// </summary>
    public static SolidColorBrush KeyBrush
        => mode == ThemeMode.Light
            ? LightKeyBrush
            : DarkKeyBrush;

    /// <summary>
    /// Gets the brush for string values for the current theme mode.
    /// </summary>
    public static SolidColorBrush StringBrush
        => mode == ThemeMode.Light
            ? LightStringBrush
            : DarkStringBrush;

    /// <summary>
    /// Gets the brush for number values for the current theme mode.
    /// </summary>
    public static SolidColorBrush NumberBrush
        => mode == ThemeMode.Light
            ? LightNumberBrush
            : DarkNumberBrush;

    /// <summary>
    /// Gets the brush for integer values for the current theme mode.
    /// </summary>
    public static SolidColorBrush IntegerBrush
        => mode == ThemeMode.Light
            ? LightIntegerBrush
            : DarkIntegerBrush;

    /// <summary>
    /// Gets the brush for floating-point values for the current theme mode.
    /// </summary>
    public static SolidColorBrush FloatBrush
        => mode == ThemeMode.Light
            ? LightFloatBrush
            : DarkFloatBrush;

    /// <summary>
    /// Gets the brush for boolean values for the current theme mode.
    /// </summary>
    public static SolidColorBrush BooleanBrush
        => mode == ThemeMode.Light
            ? LightBooleanBrush
            : DarkBooleanBrush;

    /// <summary>
    /// Gets the brush for GUID values for the current theme mode.
    /// </summary>
    public static SolidColorBrush GuidBrush
        => mode == ThemeMode.Light
            ? LightGuidBrush
            : DarkGuidBrush;

    /// <summary>
    /// Gets the brush for date values for the current theme mode.
    /// </summary>
    public static SolidColorBrush DateBrush
        => mode == ThemeMode.Light
            ? LightDateBrush
            : DarkDateBrush;

    /// <summary>
    /// Gets the brush for URI values for the current theme mode.
    /// </summary>
    public static SolidColorBrush UriBrush
        => mode == ThemeMode.Light
            ? LightUriBrush
            : DarkUriBrush;

    /// <summary>
    /// Gets the brush for null values for the current theme mode.
    /// </summary>
    public static SolidColorBrush NullBrush
        => mode == ThemeMode.Light
            ? LightNullBrush
            : DarkNullBrush;

    /// <summary>
    /// Sets the theme mode that determines which brushes are returned.
    /// </summary>
    /// <param name="themeMode">The theme mode.</param>
    public static void SetThemeMode(ThemeMode themeMode)
    {
        if (Equals(mode, themeMode))
        {
            return;
        }

        mode = themeMode;
    }

    private static SolidColorBrush CreateFrozenBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
}