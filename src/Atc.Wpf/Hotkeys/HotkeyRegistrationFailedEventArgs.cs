namespace Atc.Wpf.Hotkeys;

/// <summary>
/// Provides data for the <see cref="IHotkeyService.RegistrationFailed"/> event,
/// raised when the operating system refuses a global hotkey (typically because another
/// application already owns the same key combination).
/// </summary>
public sealed class HotkeyRegistrationFailedEventArgs(
    IHotkeyRegistration registration,
    int errorCode) : EventArgs
{
    /// <summary>
    /// Gets the registration that could not be activated. It stays in the service and does not fire.
    /// </summary>
    public IHotkeyRegistration Registration { get; } = registration;

    /// <summary>
    /// Gets the Win32 error code reported by <c>RegisterHotKey</c>
    /// (1409 = ERROR_HOTKEY_ALREADY_REGISTERED).
    /// </summary>
    public int ErrorCode { get; } = errorCode;
}