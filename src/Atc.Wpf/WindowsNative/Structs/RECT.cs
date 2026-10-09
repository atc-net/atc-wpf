namespace Atc.Wpf.WindowsNative.Structs;

/// <summary>
/// The Win32 <c>RECT</c> structure, which defines a rectangle by the coordinates of its upper-left and lower-right corners.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "OK.")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "OK.")]
public struct RECT
{
    /// <summary>The x-coordinate of the upper-left corner.</summary>
    public int Left;

    /// <summary>The y-coordinate of the upper-left corner.</summary>
    public int Top;

    /// <summary>The x-coordinate of the lower-right corner.</summary>
    public int Right;

    /// <summary>The y-coordinate of the lower-right corner.</summary>
    public int Bottom;

    /// <summary>
    /// Initializes a new instance of the <see cref="RECT"/> struct.
    /// </summary>
    /// <param name="left">The x-coordinate of the upper-left corner.</param>
    /// <param name="top">The y-coordinate of the upper-left corner.</param>
    /// <param name="right">The x-coordinate of the lower-right corner.</param>
    /// <param name="bottom">The y-coordinate of the lower-right corner.</param>
    public RECT(
        int left,
        int top,
        int right,
        int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Gets the width of the rectangle.
    /// </summary>
    public int Width => Right - Left;

    /// <summary>
    /// Gets the height of the rectangle.
    /// </summary>
    public int Height => Bottom - Top;

    /// <summary>
    /// Moves the rectangle by the specified horizontal and vertical amounts.
    /// </summary>
    /// <param name="dx">The horizontal offset.</param>
    /// <param name="dy">The vertical offset.</param>
    public void Offset(
        int dx,
        int dy)
    {
        Left += dx;
        Top += dy;
        Right += dx;
        Bottom += dy;
    }

    /// <summary>
    /// Gets a value indicating whether the rectangle has no area.
    /// </summary>
    public readonly bool IsEmpty => Left >= Right || Top >= Bottom;
}