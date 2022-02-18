namespace Atc.Wpf.WindowsNative.Structs;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "OK.")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "OK.")]
public struct POINT
{
    public int X;
    public int Y;

    public POINT(int x, int y)
    {
        X = x;
        Y = y;
    }
}