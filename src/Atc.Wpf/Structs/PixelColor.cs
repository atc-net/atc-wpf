// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable IdentifierTypo
// ReSharper disable once CheckNamespace
namespace Atc.Wpf;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct PixelColor : IEquatable<PixelColor>
{
    [FieldOffset(0)]
    public uint ColorBgra;

    [FieldOffset(0)]
    public byte Blue;

    [FieldOffset(1)]
    public byte Green;

    [FieldOffset(2)]
    public byte Red;

    [FieldOffset(3)]
    public byte Alpha;

    public override readonly string ToString()
        => $"{nameof(Blue)}: {Blue}, {nameof(Green)}: {Green}, {nameof(Red)}: {Red}, {nameof(Alpha)}: {Alpha}";

    public static bool operator ==(PixelColor pixelColor1, PixelColor pixelColor2)
        => pixelColor1.Equals(pixelColor2);

    public static bool operator !=(PixelColor pixelColor1, PixelColor pixelColor2)
        => !pixelColor1.Equals(pixelColor2);

    public readonly bool Equals(PixelColor other)
        => ColorBgra == other.ColorBgra &&
           Blue == other.Blue &&
           Green == other.Green &&
           Red == other.Red &&
           Alpha == other.Alpha;

    public override readonly bool Equals(object? obj)
        => obj is PixelColor x && Equals(x);

    [SuppressMessage("Minor Bug", "S2328:\"GetHashCode\" should not reference mutable fields", Justification = "OK.")]
    public override readonly int GetHashCode()
        => ColorBgra.GetHashCode() ^ Blue.GetHashCode() ^ Green.GetHashCode() ^ Red.GetHashCode() ^ Alpha.GetHashCode();
}