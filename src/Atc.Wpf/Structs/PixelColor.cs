// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable IdentifierTypo
// ReSharper disable once CheckNamespace
namespace Atc.Wpf;

/// <summary>
/// Represents a 32-bit BGRA pixel whose individual channels overlay the packed <see cref="ColorBgra"/> value.
/// </summary>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct PixelColor : IEquatable<PixelColor>
{
    /// <summary>The packed 32-bit BGRA value.</summary>
    [FieldOffset(0)]
    public uint ColorBgra;

    /// <summary>The blue channel.</summary>
    [FieldOffset(0)]
    public byte Blue;

    /// <summary>The green channel.</summary>
    [FieldOffset(1)]
    public byte Green;

    /// <summary>The red channel.</summary>
    [FieldOffset(2)]
    public byte Red;

    /// <summary>The alpha channel.</summary>
    [FieldOffset(3)]
    public byte Alpha;

    /// <inheritdoc />
    public override readonly string ToString()
        => $"{nameof(Blue)}: {Blue}, {nameof(Green)}: {Green}, {nameof(Red)}: {Red}, {nameof(Alpha)}: {Alpha}";

    /// <summary>
    /// Determines whether two <see cref="PixelColor"/> values are equal.
    /// </summary>
    public static bool operator ==(PixelColor pixelColor1, PixelColor pixelColor2)
        => pixelColor1.Equals(pixelColor2);

    /// <summary>
    /// Determines whether two <see cref="PixelColor"/> values are not equal.
    /// </summary>
    public static bool operator !=(PixelColor pixelColor1, PixelColor pixelColor2)
        => !pixelColor1.Equals(pixelColor2);

    /// <inheritdoc />
    public readonly bool Equals(PixelColor other)
        => ColorBgra == other.ColorBgra &&
           Blue == other.Blue &&
           Green == other.Green &&
           Red == other.Red &&
           Alpha == other.Alpha;

    /// <inheritdoc />
    public override readonly bool Equals(object? obj)
        => obj is PixelColor x && Equals(x);

    /// <inheritdoc />
    [SuppressMessage("Minor Bug", "S2328:\"GetHashCode\" should not reference mutable fields", Justification = "OK.")]
    public override readonly int GetHashCode()
        => ColorBgra.GetHashCode() ^ Blue.GetHashCode() ^ Green.GetHashCode() ^ Red.GetHashCode() ^ Alpha.GetHashCode();
}