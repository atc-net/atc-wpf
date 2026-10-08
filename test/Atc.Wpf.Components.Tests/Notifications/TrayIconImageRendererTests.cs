namespace Atc.Wpf.Components.Tests.Notifications;

public sealed class TrayIconImageRendererTests
{
    [StaFact]
    public void RenderBgra_ScalesTheImageToTheRequestedSize()
    {
        var pixels = TrayIconImageRenderer.RenderBgra(CreateSolidImage(Colors.Red, 64), 16);

        Assert.Equal(16 * 16 * 4, pixels.Length);
    }

    [StaFact]
    public void RenderBgra_ReturnsOpaquePixelsInBgraOrder()
    {
        var pixels = TrayIconImageRenderer.RenderBgra(CreateSolidImage(Colors.Red, 16), 16);

        Assert.Equal([0, 0, 255, 255], pixels[..4]);
    }

    [StaFact]
    public void RenderBgra_ReturnsStraightAlphaForSemiTransparentPixels()
    {
        var pixels = TrayIconImageRenderer.RenderBgra(CreateSolidImage(Color.FromArgb(128, 255, 0, 0), 16), 16);

        // Icons use straight (not premultiplied) alpha, so the red channel stays at full strength.
        Assert.InRange(pixels[2], 250, 255);
        Assert.InRange(pixels[3], 126, 130);
    }

    private static DrawingImage CreateSolidImage(
        Color color,
        double size)
    {
        var drawing = new GeometryDrawing(
            new SolidColorBrush(color),
            pen: null,
            new RectangleGeometry(new Rect(0, 0, size, size)));
        return new DrawingImage(drawing);
    }
}