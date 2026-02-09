namespace Atc.Wpf.Hotkeys;

/// <summary>
/// Provides MVVM-friendly keyboard shortcut management with support for
/// global (system-wide) and local (window-scoped) hotkeys, chords, and conflict detection.
/// </summary>
public interface IHotkeyService : IDisposable
{
    /// <summary>
    /// Registers a hotkey with the specified key combination.
    /// </summary>
    /// <param name="modifiers">The modifier keys (Ctrl, Alt, Shift, etc.).</param>
    /// <param name="key">The key.</param>
    /// <param name="callback">The callback to invoke when the hotkey is pressed.</param>
    /// <param name="description">An optional description for UI display.</param>
    /// <param name="scope">Whether the hotkey is global or local.</param>
    /// <returns>A registration handle that can be disposed to unregister the hotkey.</returns>
    IHotkeyRegistration Register(
        ModifierKeys modifiers,
        Key key,
        Action<HotkeyPressedEventArgs> callback,
        string? description = null,
        HotkeyScope scope = HotkeyScope.Local);

    /// <summary>
    /// Registers a two-stroke chord hotkey (e.g. Ctrl+K, Ctrl+C).
    /// Chords are always local scope.
    /// </summary>
    IHotkeyRegistration RegisterChord(
        ModifierKeys firstModifiers,
        Key firstKey,
        ModifierKeys secondModifiers,
        Key secondKey,
        Action<HotkeyPressedEventArgs> callback,
        string? description = null);

    /// <summary>
    /// Unregisters a previously registered hotkey.
    /// </summary>
    void Unregister(IHotkeyRegistration registration);

    /// <summary>
    /// Gets a read-only list of all current registrations.
    /// </summary>
    IReadOnlyList<IHotkeyRegistration> Registrations { get; }

    /// <summary>
    /// Gets a value indicating whether the service is actively listening for hotkeys.
    /// </summary>
    bool IsListening { get; }

    /// <summary>
    /// Starts listening for hotkeys on the specified window.
    /// </summary>
    void StartListening(Window window);

    /// <summary>
    /// Stops listening for hotkeys and unregisters all global hotkeys.
    /// </summary>
    void StopListening();

    /// <summary>
    /// Raised when any registered hotkey is pressed.
    /// </summary>
    event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    /// <summary>
    /// Raised when a registration conflicts with an existing registration.
    /// </summary>
    event EventHandler<HotkeyConflictEventArgs>? ConflictDetected;

    /// <summary>
    /// Saves all current hotkey bindings to a JSON file.
    /// </summary>
    /// <param name="filePath">The file path to write the JSON to.</param>
    void SaveBindings(string filePath);

    /// <summary>
    /// Loads hotkey bindings from a JSON file and registers them.
    /// Loaded bindings fire only the <see cref="HotkeyPressed"/> event;
    /// consumers should subscribe and dispatch by <see cref="IHotkeyRegistration.Description"/>.
    /// </summary>
    /// <param name="filePath">The file path to read the JSON from.</param>
    void LoadBindings(string filePath);
}