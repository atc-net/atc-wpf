namespace Atc.Wpf.Hotkeys;

/// <summary>
/// Default implementation of <see cref="IHotkeyRegistration"/>.
/// </summary>
internal sealed class HotkeyRegistration : IHotkeyRegistration
{
    private readonly Action<HotkeyRegistration>? unregisterCallback;
    private bool disposed;

    internal HotkeyRegistration(
        int id,
        ModifierKeys modifiers,
        Key key,
        Action<HotkeyPressedEventArgs> callback,
        string? description,
        HotkeyScope scope,
        HotkeyChord? chord,
        Action<HotkeyRegistration>? unregisterCallback)
    {
        Id = id;
        Modifiers = modifiers;
        Key = key;
        Callback = callback;
        Description = description;
        Scope = scope;
        Chord = chord;
        this.unregisterCallback = unregisterCallback;
    }

    public int Id { get; }

    public ModifierKeys Modifiers { get; }

    public Key Key { get; }

    public string? Description { get; }

    public HotkeyScope Scope { get; }

    public HotkeyChord? Chord { get; }

    internal Action<HotkeyPressedEventArgs> Callback { get; }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        unregisterCallback?.Invoke(this);
    }
}