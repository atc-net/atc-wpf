namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// Maps WPF <see cref="System.Windows.Input.Key"/> values to X11 keysym values
/// used by the VNC protocol.
/// </summary>
public static class KeySymMapper
{
    private static readonly Dictionary<Key, uint> KeyMap = new()
    {
        // Modifier keys
        { Key.LeftShift, 0xFFE1 },
        { Key.RightShift, 0xFFE2 },
        { Key.LeftCtrl, 0xFFE3 },
        { Key.RightCtrl, 0xFFE4 },
        { Key.LeftAlt, 0xFFE9 },
        { Key.RightAlt, 0xFFEA },
        { Key.LWin, 0xFFEB },
        { Key.RWin, 0xFFEC },
        { Key.CapsLock, 0xFFE5 },
        { Key.NumLock, 0xFF7F },
        { Key.Scroll, 0xFF14 },

        // Special keys
        { Key.Return, 0xFF0D },
        { Key.Escape, 0xFF1B },
        { Key.Tab, 0xFF09 },
        { Key.Back, 0xFF08 },
        { Key.Delete, 0xFFFF },
        { Key.Insert, 0xFF63 },
        { Key.Home, 0xFF50 },
        { Key.End, 0xFF57 },
        { Key.PageUp, 0xFF55 },
        { Key.PageDown, 0xFF56 },
        { Key.Space, 0x0020 },
        { Key.PrintScreen, 0xFF61 },
        { Key.Pause, 0xFF13 },

        // Arrow keys
        { Key.Left, 0xFF51 },
        { Key.Up, 0xFF52 },
        { Key.Right, 0xFF53 },
        { Key.Down, 0xFF54 },

        // Function keys
        { Key.F1, 0xFFBE },
        { Key.F2, 0xFFBF },
        { Key.F3, 0xFFC0 },
        { Key.F4, 0xFFC1 },
        { Key.F5, 0xFFC2 },
        { Key.F6, 0xFFC3 },
        { Key.F7, 0xFFC4 },
        { Key.F8, 0xFFC5 },
        { Key.F9, 0xFFC6 },
        { Key.F10, 0xFFC7 },
        { Key.F11, 0xFFC8 },
        { Key.F12, 0xFFC9 },

        // Numpad
        { Key.NumPad0, 0xFFB0 },
        { Key.NumPad1, 0xFFB1 },
        { Key.NumPad2, 0xFFB2 },
        { Key.NumPad3, 0xFFB3 },
        { Key.NumPad4, 0xFFB4 },
        { Key.NumPad5, 0xFFB5 },
        { Key.NumPad6, 0xFFB6 },
        { Key.NumPad7, 0xFFB7 },
        { Key.NumPad8, 0xFFB8 },
        { Key.NumPad9, 0xFFB9 },
        { Key.Multiply, 0xFFAA },
        { Key.Add, 0xFFAB },
        { Key.Subtract, 0xFFAD },
        { Key.Decimal, 0xFFAE },
        { Key.Divide, 0xFFAF },

        // Digits (top row)
        { Key.D0, 0x0030 },
        { Key.D1, 0x0031 },
        { Key.D2, 0x0032 },
        { Key.D3, 0x0033 },
        { Key.D4, 0x0034 },
        { Key.D5, 0x0035 },
        { Key.D6, 0x0036 },
        { Key.D7, 0x0037 },
        { Key.D8, 0x0038 },
        { Key.D9, 0x0039 },

        // Letters (lowercase X11 keysym)
        { Key.A, 0x0061 },
        { Key.B, 0x0062 },
        { Key.C, 0x0063 },
        { Key.D, 0x0064 },
        { Key.E, 0x0065 },
        { Key.F, 0x0066 },
        { Key.G, 0x0067 },
        { Key.H, 0x0068 },
        { Key.I, 0x0069 },
        { Key.J, 0x006A },
        { Key.K, 0x006B },
        { Key.L, 0x006C },
        { Key.M, 0x006D },
        { Key.N, 0x006E },
        { Key.O, 0x006F },
        { Key.P, 0x0070 },
        { Key.Q, 0x0071 },
        { Key.R, 0x0072 },
        { Key.S, 0x0073 },
        { Key.T, 0x0074 },
        { Key.U, 0x0075 },
        { Key.V, 0x0076 },
        { Key.W, 0x0077 },
        { Key.X, 0x0078 },
        { Key.Y, 0x0079 },
        { Key.Z, 0x007A },

        // Punctuation / symbols
        { Key.OemMinus, 0x002D },
        { Key.OemPlus, 0x003D },
        { Key.OemOpenBrackets, 0x005B },
        { Key.OemCloseBrackets, 0x005D },
        { Key.OemPipe, 0x005C },
        { Key.OemSemicolon, 0x003B },
        { Key.OemQuotes, 0x0027 },
        { Key.OemComma, 0x002C },
        { Key.OemPeriod, 0x002E },
        { Key.OemQuestion, 0x002F },
        { Key.OemTilde, 0x0060 },
    };

    public static bool TryGetKeySym(
        Key key,
        out uint keySym)
        => KeyMap.TryGetValue(key, out keySym);
}