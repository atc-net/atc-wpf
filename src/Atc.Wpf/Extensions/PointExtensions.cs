// ReSharper disable once CheckNamespace
namespace System.Windows;

public static class PointExtensions
{
    /// <summary>
    /// Clamps the point to the inside of the element
    /// </summary>
    /// <param name="point">The point</param>
    /// <param name="element">The element</param>
    /// <returns>A point inside the element</returns>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "OK - By design.")]
    public static Point Clamp(
        this Point point,
        FrameworkElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        var pos = Mouse.GetPosition(element);
        pos.X = Math.Min(Math.Max(0, pos.X), element.ActualWidth);
        pos.Y = Math.Min(Math.Max(0, pos.Y), element.ActualHeight);
        return pos;
    }
}