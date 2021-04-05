using System.Windows.Media;
using System.Windows.Media.Imaging;

// ReSharper disable once CheckNamespace
namespace System.Windows
{
    public static class UiElementExtensions
    {
        public static BitmapSource SnapShotToBitmap(this UIElement uiElement, double zoomFactor)
        {
            return uiElement.SnapShotToBitmap(zoomFactor, zoomFactor);
        }

        public static BitmapSource SnapShotToBitmap(this UIElement uiElement, double zoomFactorX, double zoomFactorY)
        {
            if (uiElement is null)
            {
                throw new ArgumentNullException(nameof(uiElement));
            }

            double actualHeight = uiElement.RenderSize.Height;
            double actualWidth = uiElement.RenderSize.Width;

            if (actualHeight < 1)
            {
                actualHeight = 1;
            }

            if (actualWidth < 1)
            {
                actualWidth = 1;
            }

            double pixelWidth = actualWidth * zoomFactorX;
            double pixelHeight = actualHeight * zoomFactorY;

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
    }
}