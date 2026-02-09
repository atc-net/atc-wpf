namespace Atc.Wpf.Hotkeys;

/// <summary>
/// Provides data for the <see cref="IHotkeyService.ConflictDetected"/> event,
/// raised when a registration conflicts with an existing one.
/// </summary>
public sealed class HotkeyConflictEventArgs(
    IHotkeyRegistration existing,
    IHotkeyRegistration requested) : EventArgs
{
    /// <summary>
    /// Gets the existing registration that conflicts.
    /// </summary>
    public IHotkeyRegistration Existing { get; } = existing;

    /// <summary>
    /// Gets the newly requested registration that caused the conflict.
    /// </summary>
    public IHotkeyRegistration Requested { get; } = requested;
}