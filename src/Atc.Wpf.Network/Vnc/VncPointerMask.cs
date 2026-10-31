namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// Builds RFB pointer-event button masks (bit 0 = left, 1 = middle, 2 = right, 3/4 = wheel up/down).
/// </summary>
internal static class VncPointerMask
{
    internal const byte Left = 1;
    internal const byte Middle = 2;
    internal const byte Right = 4;
    internal const byte WheelUp = 8;
    internal const byte WheelDown = 16;

    private const int WheelNotch = 120;

    /// <summary>
    /// Gets the mask for the buttons that are held down.
    /// </summary>
    internal static byte FromButtons(
        bool left,
        bool middle,
        bool right)
    {
        byte mask = 0;
        if (left)
        {
            mask |= Left;
        }

        if (middle)
        {
            mask |= Middle;
        }

        if (right)
        {
            mask |= Right;
        }

        return mask;
    }

    /// <summary>
    /// Gets the mask after one button changed: every held button stays down, and the changed
    /// button is forced to <paramref name="pressed"/> whatever the device state reports for it.
    /// </summary>
    internal static byte ForButtonChange(
        bool left,
        bool middle,
        bool right,
        MouseButton changedButton,
        bool pressed)
    {
        var mask = FromButtons(left, middle, right);
        var bit = changedButton switch
        {
            MouseButton.Left => Left,
            MouseButton.Middle => Middle,
            MouseButton.Right => Right,
            _ => (byte)0,
        };

        return pressed
            ? (byte)(mask | bit)
            : (byte)(mask & ~bit);
    }

    /// <summary>
    /// Maps a mouse-wheel delta to the wheel button (4 = up, 5 = down) combined with the held buttons,
    /// and the number of press/release clicks to send (one per notch, at least one for a partial notch).
    /// </summary>
    internal static (byte Mask, int Clicks) GetWheel(
        int delta,
        byte heldMask)
    {
        if (delta == 0)
        {
            return (heldMask, 0);
        }

        var bit = delta > 0 ? WheelUp : WheelDown;
        var clicks = System.Math.Max(1, System.Math.Abs(delta) / WheelNotch);
        return ((byte)(heldMask | bit), clicks);
    }
}