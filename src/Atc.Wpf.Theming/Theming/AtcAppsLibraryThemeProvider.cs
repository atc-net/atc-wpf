namespace Atc.Wpf.Theming.Theming;

/// <summary>
/// The library theme provider that supplies the Atc.Wpf.Theming themes and runtime color scheme values
/// (accent, highlight, ideal foreground and high-contrast colors) to the theme manager.
/// </summary>
public sealed class AtcAppsLibraryThemeProvider : LibraryThemeProvider
{
    /// <summary>
    /// Gets the shared instance of the provider.
    /// </summary>
    public static readonly AtcAppsLibraryThemeProvider Instance = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AtcAppsLibraryThemeProvider"/> class
    /// and registers it with the theme manager.
    /// </summary>
    public AtcAppsLibraryThemeProvider()
        : this(registerAtThemeManager: true)
    {
    }

    internal AtcAppsLibraryThemeProvider(bool registerAtThemeManager)
        : base(registerAtThemeManager)
    {
    }

    /// <inheritdoc />
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

        // These values are applied before the base color scheme's, so the high-contrast colors win.
        if (colorValues.Options.IsHighContrast &&
            colorValues.Options.BaseColorScheme is not null)
        {
            HighContrastColorScheme.Fill(
                values,
                colorValues.Options.BaseColorScheme.Values,
                HighContrastPalette.FromSystemColors());
        }
    }

    /// <inheritdoc />
    public override void PrepareRuntimeThemeResourceDictionary(
        RuntimeThemeGenerator runtimeThemeGenerator,
        ResourceDictionary resourceDictionary,
        RuntimeThemeColorValues runtimeThemeColorValues)
    {
        ArgumentNullException.ThrowIfNull(resourceDictionary);
        ArgumentNullException.ThrowIfNull(runtimeThemeColorValues);

        base.PrepareRuntimeThemeResourceDictionary(runtimeThemeGenerator, resourceDictionary, runtimeThemeColorValues);

        if (runtimeThemeColorValues.Options.IsHighContrast)
        {
            HighContrastColorScheme.MakeBordersVisible(resourceDictionary, HighContrastPalette.FromSystemColors());
        }
    }
}