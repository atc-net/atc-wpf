namespace Atc.Wpf.Theming.Internal;

internal static class PInvokeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHeight(this RECT rect)
        => rect.bottom - rect.top;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetWidth(this RECT rect)
        => rect.right - rect.left;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this RECT rect)
        => rect.left >= rect.right || rect.top >= rect.bottom;

    public static void Offset(
        this RECT rect,
        int offsetX,
        int offsetY)
    {
        rect.left += offsetX;
        rect.top += offsetY;
    }
}