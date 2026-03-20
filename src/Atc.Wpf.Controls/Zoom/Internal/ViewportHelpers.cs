namespace Atc.Wpf.Controls.Zoom.Internal;

internal static class ViewportHelpers
{
    /// <summary>
    /// Return a Rect specified by two points and clipped by a rectangle specified
    /// by two other points.
    /// </summary>
    public static Rect Clip(
        Point value1,
        Point value2,
        Point topLeft,
        Point bottomRight)
    {
        var point1 = value1.Clamp(topLeft, bottomRight);
        var point2 = value2.Clamp(topLeft, bottomRight);
        var newTopLeft = new Point(
            System.Math.Min(point1.X, point2.X),
            System.Math.Min(point1.Y, point2.Y));
        var size = new Size(
            System.Math.Abs(point1.X - point2.X),
            System.Math.Abs(point1.Y - point2.Y));

        return new Rect(newTopLeft, size);
    }

    /// <summary>
    /// Moves and sizes a border on a Canvas according to a Rect.
    /// </summary>
    public static void PositionBorderOnCanvas(
        Border border,
        Rect rect)
    {
        Canvas.SetLeft(border, rect.Left);
        Canvas.SetTop(border, rect.Top);
        border.Width = rect.Width;
        border.Height = rect.Height;
    }

    public static double FitZoom(
        double actualWidth,
        double actualHeight,
        double? contentWidth,
        double? contentHeight)
    {
        if (!contentWidth.HasValue ||
            !contentHeight.HasValue)
        {
            return 1;
        }

        return System.Math.Min(
            actualWidth / contentWidth.Value,
            actualHeight / contentHeight.Value);
    }

    public static double FillZoom(
        double actualWidth,
        double actualHeight,
        double? contentWidth,
        double? contentHeight)
    {
        if (!contentWidth.HasValue ||
            !contentHeight.HasValue)
        {
            return 1;
        }

        return System.Math.Max(
            actualWidth / contentWidth.Value,
            actualHeight / contentHeight.Value);
    }
}