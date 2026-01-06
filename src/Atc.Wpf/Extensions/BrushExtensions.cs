// ReSharper disable once CheckNamespace
namespace System.Windows.Media;

public static class BrushExtensions
{
    /// <summary>
    /// Create BitmapSource from a brush.
    /// </summary>
    /// <param name="brush">The brush.</param>
    /// <param name="pixelWidthAndHeight">The pixel width and height.</param>
    /// <param name="dpiX">The dpi x.</param>
    /// <param name="dpiY">The dpi y.</param>
    /// <returns>The BitmapSource from a brush.</returns>
    public static BitmapSource ToBitmapSource(
        this Brush brush,
        int pixelWidthAndHeight = 32,
        double dpiX = 96,
        double dpiY = 96)
        => brush.ToBitmapSource(
            pixelWidthAndHeight,
            pixelWidthAndHeight,
            dpiX,
            dpiY);

    /// <summary>
    /// Create BitmapSource from a brush.
    /// </summary>
    /// <param name="brush">The brush.</param>
    /// <param name="pixelWidth">The pixel width.</param>
    /// <param name="pixelHeight">The pixel height.</param>
    /// <param name="dpiX">The dpi x.</param>
    /// <param name="dpiY">The dpi y.</param>
    /// <returns>The BitmapSource from a brush.</returns>
    public static BitmapSource ToBitmapSource(
        this Brush brush,
        int pixelWidth = 32,
        int pixelHeight = 32,
        double dpiX = 96,
        double dpiY = 96)
    {
        ArgumentNullException.ThrowIfNull(brush);

        if (pixelWidth < 1)
        {
            pixelWidth = 1;
        }

        if (pixelHeight < 1)
        {
            pixelHeight = 1;
        }

        if (dpiX < 1)
        {
            dpiX = 96;
        }

        if (dpiY < 1)
        {
            dpiY = 96;
        }

        var renderTargetBitmap = new RenderTargetBitmap(
            pixelWidth,
            pixelHeight,
            dpiX,
            dpiY,
            PixelFormats.Pbgra32);

        var drawingVisual = new DrawingVisual();
        using (var drawingContext = drawingVisual.RenderOpen())
        {
            drawingContext.DrawRectangle(
                brush,
                pen: null,
                new Rect(0, 0, pixelWidth, pixelHeight));
        }

        renderTargetBitmap.Render(drawingVisual);
        return renderTargetBitmap;
    }

    /// <summary>
    /// Verifies if the given brush is a SolidColorBrush and
    /// its color does not include transparency.
    /// </summary>
    /// <param name="brush">Brush</param>
    /// <returns>true if yes, otherwise false</returns>
    public static bool IsOpaqueSolidColorBrush(this Brush brush)
        => (brush as SolidColorBrush)?.Color.A == 0xFF;

    /// <summary>
    /// Verifies if the given brush is the same as the otherBrush.
    /// </summary>
    /// <param name="brush">Brush</param>
    /// <param name="otherBrush">The other Brush</param>
    /// <returns>true if yes, otherwise false</returns>
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    public static bool IsEqualTo(
        this Brush brush,
        Brush otherBrush)
    {
        ArgumentNullException.ThrowIfNull(brush);
        ArgumentNullException.ThrowIfNull(otherBrush);

        if (brush.GetType() != otherBrush.GetType())
        {
            return false;
        }

        if (ReferenceEquals(brush, otherBrush))
        {
            return true;
        }

        switch (brush)
        {
            // Are both instances of SolidColorBrush
            case SolidColorBrush solidBrushA when otherBrush is SolidColorBrush solidBrushB:
                return solidBrushA.Color == solidBrushB.Color &&
                       solidBrushA.Opacity.AreClose(solidBrushB.Opacity);

            // Are both instances of LinearGradientBrush
            case LinearGradientBrush linGradBrushA when otherBrush is LinearGradientBrush linGradBrushB:
            {
                var result = linGradBrushA.ColorInterpolationMode == linGradBrushB.ColorInterpolationMode &&
                             linGradBrushA.EndPoint == linGradBrushB.EndPoint &&
                             linGradBrushA.MappingMode == linGradBrushB.MappingMode &&
                             linGradBrushA.Opacity.AreClose(linGradBrushB.Opacity) &&
                             linGradBrushA.StartPoint == linGradBrushB.StartPoint &&
                             linGradBrushA.SpreadMethod == linGradBrushB.SpreadMethod &&
                             linGradBrushA.GradientStops.Count == linGradBrushB.GradientStops.Count;
                if (!result)
                {
                    return false;
                }

                for (var i = 0; i < linGradBrushA.GradientStops.Count; i++)
                {
                    result = (linGradBrushA.GradientStops[i].Color == linGradBrushB.GradientStops[i].Color)
                             && linGradBrushA.GradientStops[i].Offset.AreClose(linGradBrushB.GradientStops[i].Offset);

                    if (!result)
                    {
                        break;
                    }
                }

                return result;
            }

            // Are both instances of RadialGradientBrush
            case RadialGradientBrush radGradBrushA when otherBrush is RadialGradientBrush radGradBrushB:
            {
                var result = radGradBrushA.ColorInterpolationMode == radGradBrushB.ColorInterpolationMode &&
                             radGradBrushA.GradientOrigin == radGradBrushB.GradientOrigin &&
                             radGradBrushA.MappingMode == radGradBrushB.MappingMode &&
                             radGradBrushA.Opacity.AreClose(radGradBrushB.Opacity) &&
                             radGradBrushA.RadiusX.AreClose(radGradBrushB.RadiusX) &&
                             radGradBrushA.RadiusY.AreClose(radGradBrushB.RadiusY) &&
                             radGradBrushA.SpreadMethod == radGradBrushB.SpreadMethod &&
                             radGradBrushA.GradientStops.Count == radGradBrushB.GradientStops.Count;
                if (!result)
                {
                    return false;
                }

                for (var i = 0; i < radGradBrushA.GradientStops.Count; i++)
                {
                    result = (radGradBrushA.GradientStops[i].Color == radGradBrushB.GradientStops[i].Color)
                             && radGradBrushA.GradientStops[i].Offset.AreClose(radGradBrushB.GradientStops[i].Offset);

                    if (!result)
                    {
                        break;
                    }
                }

                return result;
            }

            // Are both instances of ImageBrush
            case ImageBrush imgBrushA when otherBrush is ImageBrush imgBrushB:
            {
                var result = imgBrushA.AlignmentX == imgBrushB.AlignmentX &&
                             imgBrushA.AlignmentY == imgBrushB.AlignmentY &&
                             imgBrushA.Opacity.AreClose(imgBrushB.Opacity) &&
                             imgBrushA.Stretch == imgBrushB.Stretch &&
                             imgBrushA.TileMode == imgBrushB.TileMode &&
                             imgBrushA.Viewbox == imgBrushB.Viewbox &&
                             imgBrushA.ViewboxUnits == imgBrushB.ViewboxUnits &&
                             imgBrushA.Viewport == imgBrushB.Viewport &&
                             imgBrushA.ViewportUnits == imgBrushB.ViewportUnits &&
                             imgBrushA.ImageSource == imgBrushB.ImageSource;

                return result;
            }

            default:
                return false;
        }
    }
}