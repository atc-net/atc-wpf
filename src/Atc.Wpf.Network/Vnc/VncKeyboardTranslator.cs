namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// Turns WPF key and text input into RFB key events that work with any local keyboard layout.
/// </summary>
/// <remarks>
/// <para>
/// Printable keys are sent from the character they type (<c>TextInput</c>), so æ, ø, å, AltGr
/// characters and dead-key compositions arrive as typed. Text has no release event, so each typed
/// character is sent as a press and a release, and the key's own release sends nothing.
/// </para>
/// <para>
/// Non-printable keys (modifiers, navigation, function and keypad keys) are sent from the key, and
/// so are printable keys while Ctrl, Alt or Win is held (WPF reports control text then, and the
/// remote shortcut needs the key). Each key sends the same keysym on release as on press.
/// </para>
/// <para>
/// Windows reports AltGr as a synthetic LeftCtrl followed by RightAlt. The LeftCtrl already sent is
/// released, RightAlt is not sent, and the matching releases are dropped, so the server never sees
/// a stray Ctrl+Alt around the AltGr character.
/// </para>
/// </remarks>
internal sealed class VncKeyboardTranslator
{
    private readonly Dictionary<Key, uint> keysDown = [];
    private readonly HashSet<Key> suppressedReleases = [];
    private bool isAltGrDown;
    private bool expectsText;

    /// <summary>
    /// Handles a key press. An empty result means the key is sent from the text it types.
    /// </summary>
    internal IReadOnlyList<VncKeyEvent> KeyDown(Key key)
    {
        if (isAltGrDown && key is Key.LeftCtrl or Key.RightAlt)
        {
            // Auto-repeat of the held AltGr pair.
            return [];
        }

        if (key == Key.RightAlt &&
            keysDown.Remove(Key.LeftCtrl, out var ctrlKeySym))
        {
            isAltGrDown = true;
            expectsText = false;
            suppressedReleases.Add(Key.LeftCtrl);
            suppressedReleases.Add(Key.RightAlt);
            return [new VncKeyEvent(ctrlKeySym, Pressed: false)];
        }

        if (IsTextKey(key) && !IsShortcutModifierDown())
        {
            expectsText = true;
            return [];
        }

        expectsText = false;

        if (keysDown.TryGetValue(key, out var keySym) ||
            KeySymMapper.TryGetKeySym(key, out keySym))
        {
            keysDown[key] = keySym;
            return [new VncKeyEvent(keySym, Pressed: true)];
        }

        return [];
    }

    /// <summary>
    /// Handles a key release.
    /// </summary>
    internal IReadOnlyList<VncKeyEvent> KeyUp(Key key)
    {
        if (suppressedReleases.Remove(key))
        {
            if (key == Key.RightAlt)
            {
                isAltGrDown = false;
            }

            return [];
        }

        return keysDown.Remove(key, out var keySym)
            ? [new VncKeyEvent(keySym, Pressed: false)]
            : [];
    }

    /// <summary>
    /// Handles the text typed by the last key press.
    /// </summary>
    internal IReadOnlyList<VncKeyEvent> TextInput(string? text)
    {
        if (!expectsText || string.IsNullOrEmpty(text))
        {
            return [];
        }

        expectsText = false;

        var events = new List<VncKeyEvent>(2);
        foreach (var rune in text.EnumerateRunes())
        {
            if (KeySymMapper.TryGetKeySym(rune, out var keySym))
            {
                events.Add(new VncKeyEvent(keySym, Pressed: true));
                events.Add(new VncKeyEvent(keySym, Pressed: false));
            }
        }

        return events;
    }

    /// <summary>
    /// Releases every key still down on the server, e.g. when the view loses keyboard focus.
    /// </summary>
    internal IReadOnlyList<VncKeyEvent> ReleaseAll()
    {
        var events = new List<VncKeyEvent>(keysDown.Count);
        foreach (var keySym in keysDown.Values)
        {
            events.Add(new VncKeyEvent(keySym, Pressed: false));
        }

        keysDown.Clear();
        suppressedReleases.Clear();
        isAltGrDown = false;
        expectsText = false;
        return events;
    }

    private bool IsShortcutModifierDown()
        => keysDown.ContainsKey(Key.LeftCtrl) ||
           keysDown.ContainsKey(Key.RightCtrl) ||
           keysDown.ContainsKey(Key.LeftAlt) ||
           keysDown.ContainsKey(Key.RightAlt) ||
           keysDown.ContainsKey(Key.LWin) ||
           keysDown.ContainsKey(Key.RWin);

    private static bool IsTextKey(Key key)
        => key is (>= Key.A and <= Key.Z)
            or (>= Key.D0 and <= Key.D9)
            or (>= Key.OemSemicolon and <= Key.OemBackslash)
            or Key.Space;
}