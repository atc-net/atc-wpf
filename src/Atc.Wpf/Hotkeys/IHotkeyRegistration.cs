namespace Atc.Wpf.Hotkeys;

/// <summary>
/// Represents a handle to a registered hotkey.
/// Disposing the registration unregisters the hotkey.
/// </summary>
public interface IHotkeyRegistration : IDisposable
{
    /// <summary>
    /// Gets the unique identifier for this registration.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Gets the modifier keys for this hotkey (or the first stroke of a chord).
    /// </summary>
    ModifierKeys Modifiers { get; }

    /// <summary>
    /// Gets the key for this hotkey (or the first stroke of a chord).
    /// </summary>
    Key Key { get; }

    /// <summary>
    /// Gets the optional description for display in UI.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Gets the scope of this registration.
    /// </summary>
    HotkeyScope Scope { get; }

    /// <summary>
    /// Gets the chord definition if this is a chord registration; otherwise <c>null</c>.
    /// </summary>
    HotkeyChord? Chord { get; }
}