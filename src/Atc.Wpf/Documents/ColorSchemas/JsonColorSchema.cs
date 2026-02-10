namespace Atc.Wpf.Documents.ColorSchemas;

public static class JsonColorSchema
{
    // Light Theme Colors
    private static readonly SolidColorBrush LightDefaultBrush = new(Colors.Black);
    private static readonly SolidColorBrush LightKeyBrush = new(Color.FromRgb(64, 101, 195));
    private static readonly SolidColorBrush LightStringBrush = new(Color.FromRgb(210, 99, 40));
    private static readonly SolidColorBrush LightNumberBrush = new(Color.FromRgb(76, 175, 80));
    private static readonly SolidColorBrush LightIntegerBrush = new(Color.FromRgb(76, 175, 80));
    private static readonly SolidColorBrush LightFloatBrush = new(Color.FromRgb(76, 175, 80));
    private static readonly SolidColorBrush LightBooleanBrush = new(Color.FromRgb(218, 165, 30));
    private static readonly SolidColorBrush LightGuidBrush = new(Color.FromRgb(156, 39, 176));
    private static readonly SolidColorBrush LightDateBrush = new(Color.FromRgb(121, 134, 203));
    private static readonly SolidColorBrush LightUriBrush = new(Color.FromRgb(77, 140, 183));
    private static readonly SolidColorBrush LightNullBrush = new(Colors.Gray);

    // Dark Theme Colors
    private static readonly SolidColorBrush DarkDefaultBrush = new(Colors.White);
    private static readonly SolidColorBrush DarkKeyBrush = new(Color.FromRgb(79, 195, 247));
    private static readonly SolidColorBrush DarkStringBrush = new(Color.FromRgb(255, 152, 0));
    private static readonly SolidColorBrush DarkNumberBrush = new(Color.FromRgb(139, 195, 74));
    private static readonly SolidColorBrush DarkIntegerBrush = new(Color.FromRgb(139, 195, 74));
    private static readonly SolidColorBrush DarkFloatBrush = new(Color.FromRgb(139, 195, 74));
    private static readonly SolidColorBrush DarkBooleanBrush = new(Color.FromRgb(255, 220, 6));
    private static readonly SolidColorBrush DarkGuidBrush = new(Color.FromRgb(206, 147, 216));
    private static readonly SolidColorBrush DarkDateBrush = new(Color.FromRgb(121, 134, 203));
    private static readonly SolidColorBrush DarkUriBrush = new(Color.FromRgb(86, 156, 203));
    private static readonly SolidColorBrush DarkNullBrush = new(Color.FromRgb(158, 158, 158));

    private static ThemeMode mode = ThemeMode.Light;

    public static SolidColorBrush DefaultBrush
        => mode == ThemeMode.Light
            ? LightDefaultBrush
            : DarkDefaultBrush;

    public static SolidColorBrush KeyBrush
        => mode == ThemeMode.Light
            ? LightKeyBrush
            : DarkKeyBrush;

    public static SolidColorBrush StringBrush
        => mode == ThemeMode.Light
            ? LightStringBrush
            : DarkStringBrush;

    public static SolidColorBrush NumberBrush
        => mode == ThemeMode.Light
            ? LightNumberBrush
            : DarkNumberBrush;

    public static SolidColorBrush IntegerBrush
        => mode == ThemeMode.Light
            ? LightIntegerBrush
            : DarkIntegerBrush;

    public static SolidColorBrush FloatBrush
        => mode == ThemeMode.Light
            ? LightFloatBrush
            : DarkFloatBrush;

    public static SolidColorBrush BooleanBrush
        => mode == ThemeMode.Light
            ? LightBooleanBrush
            : DarkBooleanBrush;

    public static SolidColorBrush GuidBrush
        => mode == ThemeMode.Light
            ? LightGuidBrush
            : DarkGuidBrush;

    public static SolidColorBrush DateBrush
        => mode == ThemeMode.Light
            ? LightDateBrush
            : DarkDateBrush;

    public static SolidColorBrush UriBrush
        => mode == ThemeMode.Light
            ? LightUriBrush
            : DarkUriBrush;

    public static SolidColorBrush NullBrush
        => mode == ThemeMode.Light
            ? LightNullBrush
            : DarkNullBrush;

    public static void SetThemeMode(ThemeMode themeMode)
    {
        if (Equals(mode, themeMode))
        {
            return;
        }

        mode = themeMode;
    }
}