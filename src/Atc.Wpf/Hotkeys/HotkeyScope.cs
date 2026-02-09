namespace Atc.Wpf.Hotkeys;

/// <summary>
/// Specifies whether a hotkey registration is global (system-wide) or local (window-scoped).
/// </summary>
public enum HotkeyScope
{
    /// <summary>
    /// The hotkey is active only within the associated window.
    /// </summary>
    Local,

    /// <summary>
    /// The hotkey is active system-wide via Win32 RegisterHotKey.
    /// </summary>
    Global,
}