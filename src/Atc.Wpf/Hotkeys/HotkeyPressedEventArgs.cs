namespace Atc.Wpf.Hotkeys;

/// <summary>
/// Provides data for the <see cref="IHotkeyService.HotkeyPressed"/> event.
/// </summary>
public sealed class HotkeyPressedEventArgs(IHotkeyRegistration registration) : EventArgs
{
    /// <summary>
    /// Gets the registration that was triggered.
    /// </summary>
    public IHotkeyRegistration Registration { get; } = registration;

    /// <summary>
    /// Gets or sets a value indicating whether the hotkey press has been handled.
    /// </summary>
    public bool Handled { get; set; }
}