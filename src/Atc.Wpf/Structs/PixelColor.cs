// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable IdentifierTypo
// ReSharper disable once CheckNamespace
namespace Atc.Wpf;

[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "OK.")]
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

    public override string ToString()
        => $"{nameof(Blue)}: {this.Blue}, {nameof(Green)}: {this.Green}, {nameof(Red)}: {this.Red}, {nameof(Alpha)}: {this.Alpha}";

    public static bool operator ==(PixelColor pixelColor1, PixelColor pixelColor2)
        => pixelColor1.Equals(pixelColor2);

    public static bool operator !=(PixelColor pixelColor1, PixelColor pixelColor2)
        => !pixelColor1.Equals(pixelColor2);

    public bool Equals(PixelColor other)
        => this.ColorBgra == other.ColorBgra &&
           this.Blue == other.Blue &&
           this.Green == other.Green &&
           this.Red == other.Red &&
           this.Alpha == other.Alpha;

    public override bool Equals(object? obj)
        => obj is PixelColor x && Equals(x);

    [SuppressMessage("Minor Bug", "S2328:\"GetHashCode\" should not reference mutable fields", Justification = "OK.")]
    public override int GetHashCode()
        => this.ColorBgra.GetHashCode() ^ this.Blue.GetHashCode() ^ this.Green.GetHashCode() ^ this.Red.GetHashCode() ^ this.Alpha.GetHashCode();
}