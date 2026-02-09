namespace Atc.Wpf.Hotkeys;

/// <summary>
/// A serializable definition of a hotkey binding for persistence.
/// When <see cref="SecondModifiers"/> and <see cref="SecondKey"/> are non-null,
/// the binding represents a two-stroke chord.
/// </summary>
public sealed class HotkeyBindingDefinition
{
    public ModifierKeys Modifiers { get; set; }

    public Key Key { get; set; }

    public string? Description { get; set; }

    public HotkeyScope Scope { get; set; }

    public ModifierKeys? SecondModifiers { get; set; }

    public Key? SecondKey { get; set; }
}