// ReSharper disable once CheckNamespace
// ReSharper disable ConvertIfStatementToSwitchStatement
namespace System.Windows;

public static class UiElementExtensions
{
    public static BitmapSource SnapShotToBitmap(
        this UIElement uiElement,
        double zoomFactor)
        => uiElement.SnapShotToBitmap(zoomFactor, zoomFactor);

    public static BitmapSource SnapShotToBitmap(
        this UIElement uiElement,
        double zoomFactorX,
        double zoomFactorY)
    {
        ArgumentNullException.ThrowIfNull(uiElement);

        var actualHeight = uiElement.RenderSize.Height;
        var actualWidth = uiElement.RenderSize.Width;

        if (actualHeight < 1)
        {
            actualHeight = 1;
        }

        if (actualWidth < 1)
        {
            actualWidth = 1;
        }

        var pixelWidth = actualWidth * zoomFactorX;
        var pixelHeight = actualHeight * zoomFactorY;

        var renderTargetBitmap = new RenderTargetBitmap(
            (int)pixelWidth,
            (int)pixelHeight,
            96,
            96,
            PixelFormats.Pbgra32);

        var visualBrush = new VisualBrush(uiElement);
        var drawingVisual = new DrawingVisual();
        var drawingContext = drawingVisual.RenderOpen();

        using (drawingContext)
        {
            drawingContext.PushTransform(
                new ScaleTransform(zoomFactorX, zoomFactorY));

            drawingContext.DrawRectangle(
                visualBrush,
                pen: null,
                new Rect(
                    new Point(0, 0),
                    new Point(actualWidth, actualHeight)));
        }

        renderTargetBitmap.Render(drawingVisual);
        return renderTargetBitmap;
    }

    /// <summary>
    /// Tries to locate a given item within the visual tree,
    /// starting with the dependency object at a given position.
    /// </summary>
    /// <typeparam name="T">The type of the element to be found
    /// on the visual tree of the element at the given location.</typeparam>
    /// <param name="uiElement">The main element which is used to perform
    /// hit testing.</param>
    /// <param name="point">The position to be evaluated on the origin.</param>
    public static T? TryFindParentFromPoint<T>(
        this UIElement uiElement,
        Point point)
        where T : DependencyObject
    {
        ArgumentNullException.ThrowIfNull(uiElement);

        // ReSharper disable once UseNegatedPatternMatching
        // ReSharper disable once SuspiciousTypeConversion.Global
        var element = uiElement.InputHitTest(point) as DependencyObject;
        if (element is null)
        {
            return null;
        }

        if (element is T theObject)
        {
            return theObject;
        }

        return element.TryFindParent<T>();
    }
}