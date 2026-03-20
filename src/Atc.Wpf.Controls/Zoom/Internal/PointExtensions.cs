namespace Atc.Wpf.Controls.Zoom.Internal;

internal static class PointExtensions
{
    /// <summary>
    /// Limits the extent of a Point to the area where X and Y are at least 0.
    /// </summary>
    public static Point Clamp(this Point value)
        => new(
            System.Math.Max(value.X, 0),
            System.Math.Max(value.Y, 0));

    /// <summary>
    /// Limits the extent of a Point to the area between two points.
    /// </summary>
    public static Point Clamp(
        this Point value,
        Point topLeft,
        Point bottomRight)
        => new(
            System.Math.Max(System.Math.Min(value.X, bottomRight.X), topLeft.X),
            System.Math.Max(System.Math.Min(value.Y, bottomRight.Y), topLeft.Y));

    /// <summary>
    /// Limits the extent of a Point to the area where X and Y are at least 0 and the X and
    /// Y values specified, returning null if Point is outside this area.
    /// </summary>
    public static Point? FilterClamp(
        this Point value,
        double xMax,
        double yMax)
        => value.X < 0 || value.X > xMax || value.Y < 0 || value.Y > yMax
            ? null
            : value;
}