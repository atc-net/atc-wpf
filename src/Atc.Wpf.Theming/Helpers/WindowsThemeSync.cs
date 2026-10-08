namespace Atc.Wpf.Theming.Helpers;

/// <summary>
/// Makes the application theme follow the Windows light/dark app mode, accent color and high contrast.
/// </summary>
/// <remarks>
/// Built on the ControlzEx <see cref="ThemeManager"/>, which listens for Windows personalization
/// changes and re-applies the theme while a sync mode is active.
/// </remarks>
public static class WindowsThemeSync
{
    /// <summary>
    /// Occurs when <see cref="Mode"/> changes.
    /// </summary>
    public static event EventHandler? ModeChanged;

    /// <summary>
    /// Gets or sets which Windows settings the theme follows.
    /// Turning a mode on applies the current Windows setting immediately.
    /// </summary>
    public static WindowsThemeSyncMode Mode
    {
        get => FromThemeSyncMode(ThemeManager.Current.ThemeSyncMode);
        set
        {
            var previous = Mode;
            var themeSyncMode = ToThemeSyncMode(value);
            if (ThemeManager.Current.ThemeSyncMode == themeSyncMode)
            {
                return;
            }

            ThemeManager.Current.ThemeSyncMode = themeSyncMode;

            // Only a part that is newly turned on is applied; stopping a part leaves the theme as it is,
            // so a theme picked by hand is not replaced. Changing the theme needs an Application.
            if ((value & ~previous) != WindowsThemeSyncMode.None &&
                Application.Current is not null)
            {
                EnsureThemeGenerationIsReady();
                ThemeManager.Current.SyncTheme();
            }

            ModeChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets a value indicating whether Windows is set to light app mode.
    /// </summary>
    public static bool IsWindowsAppModeLight
        => WindowsThemeHelper.AppsUseLightTheme();

    /// <summary>
    /// Gets a value indicating whether Windows high contrast is on.
    /// </summary>
    public static bool IsWindowsHighContrast
        => SystemParameters.HighContrast;

    /// <summary>
    /// Gets the Windows accent color, or <see langword="null"/> when it cannot be read.
    /// </summary>
    public static Color? WindowsAccentColor
        => WindowsThemeHelper.GetWindowsAccentColor();

    /// <summary>
    /// Stops following the given part, for example when the user picks a theme or accent by hand.
    /// </summary>
    /// <param name="mode">The part to stop following.</param>
    public static void Stop(WindowsThemeSyncMode mode)
        => Mode &= ~mode;

    /// <summary>
    /// Following the accent color generates a theme from Theme.Template.xaml, which needs the Atc theme
    /// provider registered and Atc.Wpf loaded (the template uses its StaticResource markup type).
    /// </summary>
    private static void EnsureThemeGenerationIsReady()
    {
        _ = Theming.AtcAppsLibraryThemeProvider.Instance;
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
    }

    internal static ThemeSyncMode ToThemeSyncMode(WindowsThemeSyncMode mode)
    {
        var result = ThemeSyncMode.DoNotSync;
        if (mode.HasFlag(WindowsThemeSyncMode.AppMode))
        {
            result |= ThemeSyncMode.SyncWithAppMode;
        }

        if (mode.HasFlag(WindowsThemeSyncMode.Accent))
        {
            result |= ThemeSyncMode.SyncWithAccent;
        }

        if (mode.HasFlag(WindowsThemeSyncMode.HighContrast))
        {
            result |= ThemeSyncMode.SyncWithHighContrast;
        }

        return result;
    }

    internal static WindowsThemeSyncMode FromThemeSyncMode(ThemeSyncMode mode)
    {
        var result = WindowsThemeSyncMode.None;
        if (mode.HasFlag(ThemeSyncMode.SyncWithAppMode))
        {
            result |= WindowsThemeSyncMode.AppMode;
        }

        if (mode.HasFlag(ThemeSyncMode.SyncWithAccent))
        {
            result |= WindowsThemeSyncMode.Accent;
        }

        if (mode.HasFlag(ThemeSyncMode.SyncWithHighContrast))
        {
            result |= WindowsThemeSyncMode.HighContrast;
        }

        return result;
    }
}