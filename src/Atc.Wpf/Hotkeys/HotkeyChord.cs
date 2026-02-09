namespace Atc.Wpf.Hotkeys;

/// <summary>
/// Represents a two-stroke keyboard chord sequence (e.g. Ctrl+K, Ctrl+C).
/// </summary>
public sealed class HotkeyChord : IEquatable<HotkeyChord>
{
    public HotkeyChord(
        ModifierKeys firstModifiers,
        Key firstKey,
        ModifierKeys secondModifiers,
        Key secondKey)
    {
        FirstModifiers = firstModifiers;
        FirstKey = firstKey;
        SecondModifiers = secondModifiers;
        SecondKey = secondKey;
    }

    /// <summary>
    /// Gets the modifier keys for the first stroke.
    /// </summary>
    public ModifierKeys FirstModifiers { get; }

    /// <summary>
    /// Gets the key for the first stroke.
    /// </summary>
    public Key FirstKey { get; }

    /// <summary>
    /// Gets the modifier keys for the second stroke.
    /// </summary>
    public ModifierKeys SecondModifiers { get; }

    /// <summary>
    /// Gets the key for the second stroke.
    /// </summary>
    public Key SecondKey { get; }

    public bool Equals(HotkeyChord? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return FirstModifiers == other.FirstModifiers &&
               FirstKey == other.FirstKey &&
               SecondModifiers == other.SecondModifiers &&
               SecondKey == other.SecondKey;
    }

    public override bool Equals(object? obj)
        => Equals(obj as HotkeyChord);

    public override int GetHashCode()
        => HashCode.Combine(FirstModifiers, FirstKey, SecondModifiers, SecondKey);

    public override string ToString()
        => $"{FormatKeyCombo(FirstModifiers, FirstKey)}, {FormatKeyCombo(SecondModifiers, SecondKey)}";

    private static string FormatKeyCombo(
        ModifierKeys modifiers,
        Key key)
    {
        var parts = new List<string>(4);

        if (modifiers.HasFlag(ModifierKeys.Control))
        {
            parts.Add("Ctrl");
        }

        if (modifiers.HasFlag(ModifierKeys.Alt))
        {
            parts.Add("Alt");
        }

        if (modifiers.HasFlag(ModifierKeys.Shift))
        {
            parts.Add("Shift");
        }

        if (modifiers.HasFlag(ModifierKeys.Windows))
        {
            parts.Add("Win");
        }

        parts.Add(key.ToString());

        return string.Join('+', parts);
    }
}