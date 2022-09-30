namespace Atc.Wpf.Extensions.Internal;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[StructLayout(LayoutKind.Explicit)]
internal struct NanUnion
{
    [FieldOffset(0)]
    internal double DoubleValue;

    [FieldOffset(0)]
    internal ulong UintValue;
}