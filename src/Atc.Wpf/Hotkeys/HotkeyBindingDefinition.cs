namespace Atc.Wpf.Hotkeys;

/// <summary>
/// A serializable definition of a hotkey binding for persistence.
/// When <see cref="SecondModifiers"/> and <see cref="SecondKey"/> are non-null,
/// the binding represents a two-stroke chord.
/// </summary>
public sealed class HotkeyBindingDefinition
{
    /// <summary>
    /// Gets or sets the modifier keys of the (first) stroke.
    /// </summary>
    public ModifierKeys Modifiers { get; set; }

    /// <summary>
    /// Gets or sets the key of the (first) stroke.
    /// </summary>
    public Key Key { get; set; }

    /// <summary>
    /// Gets or sets an optional description for UI display.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether the hotkey is global or local.
    /// </summary>
    public HotkeyScope Scope { get; set; }

    /// <summary>
    /// Gets or sets the modifier keys of the second stroke of a chord, or <see langword="null"/> for a single-stroke hotkey.
    /// </summary>
    public ModifierKeys? SecondModifiers { get; set; }

    /// <summary>
    /// Gets or sets the key of the second stroke of a chord, or <see langword="null"/> for a single-stroke hotkey.
    /// </summary>
    public Key? SecondKey { get; set; }
}