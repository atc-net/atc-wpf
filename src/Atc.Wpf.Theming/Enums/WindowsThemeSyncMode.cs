// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Theming;

/// <summary>
/// Which Windows personalization settings the application theme follows.
/// </summary>
[Flags]
public enum WindowsThemeSyncMode
{
    /// <summary>
    /// The theme does not follow Windows.
    /// </summary>
    None = 0,

    /// <summary>
    /// Follow the Windows light/dark app mode.
    /// </summary>
    AppMode = 1,

    /// <summary>
    /// Follow the Windows accent color.
    /// </summary>
    Accent = 2,

    /// <summary>
    /// Follow both the Windows app mode and accent color.
    /// </summary>
    AppModeAndAccent = AppMode | Accent,

    /// <summary>
    /// Switch to a high-contrast theme built from the Windows system colors while Windows high contrast is on.
    /// </summary>
    HighContrast = 4,

    /// <summary>
    /// Follow the Windows app mode, accent color and high contrast.
    /// </summary>
    All = AppMode | Accent | HighContrast,
}