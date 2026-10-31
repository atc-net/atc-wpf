namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// One RFB key event: an X11 keysym pressed or released.
/// </summary>
/// <param name="KeySym">The X11 keysym.</param>
/// <param name="Pressed"><see langword="true"/> for a press; <see langword="false"/> for a release.</param>
internal readonly record struct VncKeyEvent(
    uint KeySym,
    bool Pressed);