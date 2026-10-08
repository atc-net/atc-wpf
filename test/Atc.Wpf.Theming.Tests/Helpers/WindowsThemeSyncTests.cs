namespace Atc.Wpf.Theming.Tests.Helpers;

/// <summary>
/// ThemeManager is process-wide, so every test restores the sync mode it found.
/// Without an Application the theme itself is never changed.
/// </summary>
public sealed class WindowsThemeSyncTests : IDisposable
{
    private readonly ThemeSyncMode original = ThemeManager.Current.ThemeSyncMode;

    public void Dispose()
        => ThemeManager.Current.ThemeSyncMode = original;

    [Theory]
    [InlineData(WindowsThemeSyncMode.None, ThemeSyncMode.DoNotSync)]
    [InlineData(WindowsThemeSyncMode.AppMode, ThemeSyncMode.SyncWithAppMode)]
    [InlineData(WindowsThemeSyncMode.Accent, ThemeSyncMode.SyncWithAccent)]
    [InlineData(WindowsThemeSyncMode.AppModeAndAccent, ThemeSyncMode.SyncWithAppMode | ThemeSyncMode.SyncWithAccent)]
    public void ToThemeSyncMode_MapsEachMode(
        WindowsThemeSyncMode mode,
        ThemeSyncMode expected)
        => Assert.Equal(expected, WindowsThemeSync.ToThemeSyncMode(mode));

    [Theory]
    [InlineData(ThemeSyncMode.DoNotSync, WindowsThemeSyncMode.None)]
    [InlineData(ThemeSyncMode.SyncWithAppMode, WindowsThemeSyncMode.AppMode)]
    [InlineData(ThemeSyncMode.SyncWithAccent, WindowsThemeSyncMode.Accent)]
    [InlineData(ThemeSyncMode.SyncAll, WindowsThemeSyncMode.AppModeAndAccent)]
    [InlineData(ThemeSyncMode.SyncWithHighContrast, WindowsThemeSyncMode.None)]
    public void FromThemeSyncMode_MapsEachModeAndIgnoresHighContrast(
        ThemeSyncMode mode,
        WindowsThemeSyncMode expected)
        => Assert.Equal(expected, WindowsThemeSync.FromThemeSyncMode(mode));

    [Fact]
    public void Mode_Set_IsAppliedToTheThemeManager()
    {
        WindowsThemeSync.Mode = WindowsThemeSyncMode.AppModeAndAccent;

        Assert.Equal(ThemeSyncMode.SyncWithAppMode | ThemeSyncMode.SyncWithAccent, ThemeManager.Current.ThemeSyncMode);
        Assert.Equal(WindowsThemeSyncMode.AppModeAndAccent, WindowsThemeSync.Mode);
    }

    [Fact]
    public void Mode_Changed_RaisesModeChangedOnce()
    {
        WindowsThemeSync.Mode = WindowsThemeSyncMode.None;
        var raised = 0;
        EventHandler handler = (_, _) => raised++;
        WindowsThemeSync.ModeChanged += handler;
        try
        {
            WindowsThemeSync.Mode = WindowsThemeSyncMode.Accent;
            WindowsThemeSync.Mode = WindowsThemeSyncMode.Accent;
        }
        finally
        {
            WindowsThemeSync.ModeChanged -= handler;
        }

        Assert.Equal(1, raised);
    }

    [Fact]
    public void Stop_RemovesOnlyTheGivenPart()
    {
        WindowsThemeSync.Mode = WindowsThemeSyncMode.AppModeAndAccent;

        WindowsThemeSync.Stop(WindowsThemeSyncMode.AppMode);

        Assert.Equal(WindowsThemeSyncMode.Accent, WindowsThemeSync.Mode);
    }
}