namespace Atc.Wpf.Theming.Theming;

public sealed class AtcAppsLibraryThemeProvider : LibraryThemeProvider
{
    public static readonly AtcAppsLibraryThemeProvider DefaultInstance = new();

    public AtcAppsLibraryThemeProvider()
        : base(registerAtThemeManager: true)
    {
    }

    public override void FillColorSchemeValues(
        Dictionary<string, string> values,
        RuntimeThemeColorValues colorValues)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(colorValues);

        values.Add("AtcApps.Colors.AccentBase", colorValues.AccentBaseColor.ToString(GlobalizationConstants.EnglishCultureInfo));
        values.Add("AtcApps.Colors.Accent", colorValues.AccentColor80.ToString(GlobalizationConstants.EnglishCultureInfo));
        values.Add("AtcApps.Colors.Accent2", colorValues.AccentColor60.ToString(GlobalizationConstants.EnglishCultureInfo));
        values.Add("AtcApps.Colors.Accent3", colorValues.AccentColor40.ToString(GlobalizationConstants.EnglishCultureInfo));
        values.Add("AtcApps.Colors.Accent4", colorValues.AccentColor20.ToString(GlobalizationConstants.EnglishCultureInfo));

        values.Add("AtcApps.Colors.Highlight", colorValues.HighlightColor.ToString(GlobalizationConstants.EnglishCultureInfo));
        values.Add("AtcApps.Colors.IdealForeground", colorValues.IdealForegroundColor.ToString(GlobalizationConstants.EnglishCultureInfo));
    }
}