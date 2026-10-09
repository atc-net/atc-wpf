namespace Atc.Wpf.WindowsNative.Structs;

/// <summary>
/// The Win32 <c>POINT</c> structure, which defines the x- and y-coordinates of a point.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "OK.")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "OK.")]
public struct POINT
{
    /// <summary>The x-coordinate of the point.</summary>
    public int X;

    /// <summary>The y-coordinate of the point.</summary>
    public int Y;

    /// <summary>
    /// Initializes a new instance of the <see cref="POINT"/> struct.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public POINT(
        int x,
        int y)
    {
        X = x;
        Y = y;
    }
}