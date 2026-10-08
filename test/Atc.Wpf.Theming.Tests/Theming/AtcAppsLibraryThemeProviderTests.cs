namespace Atc.Wpf.Theming.Tests.Theming;

public sealed class AtcAppsLibraryThemeProviderTests
{
    public AtcAppsLibraryThemeProviderTests()
    {
        // Theme.Template.xaml uses the StaticResource markup type from Atc.Wpf.
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;

        // The provider loads its themes from pack://application URIs, which the Application type
        // registers in its static constructor; no Application instance is needed.
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    [StaFact]
    public void GenerateRuntimeLibraryTheme_HighContrast_UsesTheWindowsHighContrastColors()
    {
        var libraryTheme = RuntimeThemeGenerator.Current.GenerateRuntimeLibraryTheme(
            "Dark",
            Colors.Orange,
            isHighContrast: true,
            new AtcAppsLibraryThemeProvider(registerAtThemeManager: false));

        Assert.NotNull(libraryTheme);
        Assert.True(libraryTheme.IsHighContrast);
        var resources = libraryTheme.Resources;
        Assert.Equal(SystemColors.WindowColor, BrushColor(resources, "AtcApps.Brushes.ThemeBackground"));
        Assert.Equal(SystemColors.WindowTextColor, BrushColor(resources, "AtcApps.Brushes.Text"));
        Assert.Equal(SystemColors.HighlightColor, BrushColor(resources, "AtcApps.Brushes.Accent"));
        Assert.Equal(SystemColors.WindowTextColor, BrushColor(resources, "AtcApps.Brushes.ListBox.Border"));
    }

    [StaFact]
    public void GenerateRuntimeLibraryTheme_Normal_KeepsTheThemeColors()
    {
        var libraryTheme = RuntimeThemeGenerator.Current.GenerateRuntimeLibraryTheme(
            "Dark",
            Colors.Orange,
            isHighContrast: false,
            new AtcAppsLibraryThemeProvider(registerAtThemeManager: false));

        Assert.NotNull(libraryTheme);
        Assert.False(libraryTheme.IsHighContrast);
        Assert.Equal(Color.FromRgb(0x1E, 0x1E, 0x1E), BrushColor(libraryTheme.Resources, "AtcApps.Brushes.ThemeBackground"));
    }

    private static Color BrushColor(
        ResourceDictionary resources,
        string key)
        => Assert.IsType<SolidColorBrush>(resources[key]).Color;
}