// ReSharper disable once CheckNamespace
namespace System.Windows;

public static class RectExtensions
{
    /// <summary>
    /// Deflates rectangle by given thickness
    /// </summary>
    /// <param name="rect">Rectangle</param>
    /// <param name="thickness">Thickness</param>
    /// <returns>Deflated Rectangle</returns>
    public static Rect Deflate(
        this Rect rect,
        Thickness thickness)
        => new(
            rect.Left + thickness.Left,
            rect.Top + thickness.Top,
            System.Math.Max(0.0, rect.Width - thickness.Left - thickness.Right),
            System.Math.Max(0.0, rect.Height - thickness.Top - thickness.Bottom));

    /// <summary>
    /// Inflates rectangle by given thickness
    /// </summary>
    /// <param name="rect">Rectangle</param>
    /// <param name="thickness">Thickness</param>
    /// <returns>Inflated Rectangle</returns>
    public static Rect Inflate(
        this Rect rect,
        Thickness thickness)
        => new(
            rect.Left - thickness.Left,
            rect.Top - thickness.Top,
            System.Math.Max(0.0, rect.Width + thickness.Left + thickness.Right),
            System.Math.Max(0.0, rect.Height + thickness.Top + thickness.Bottom));
}