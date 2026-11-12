namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// Maps a mouse position on the shown image to a position on the remote framebuffer.
/// </summary>
public static class VncPointerMapping
{
    /// <summary>
    /// Maps a position on the image to the framebuffer, for the given scale mode.
    /// </summary>
    /// <param name="position">The mouse position, relative to the image element.</param>
    /// <param name="imageSize">The image element's actual size.</param>
    /// <param name="framebufferWidth">The framebuffer width in pixels.</param>
    /// <param name="framebufferHeight">The framebuffer height in pixels.</param>
    /// <param name="scaleMode">How the image is scaled.</param>
    /// <returns>The framebuffer position, clamped to the framebuffer.</returns>
    public static (int X, int Y) MapToFramebuffer(
        Point position,
        Size imageSize,
        int framebufferWidth,
        int framebufferHeight,
        VncScaleMode scaleMode)
    {
        if (framebufferWidth <= 0 || framebufferHeight <= 0)
        {
            return (-1, -1);
        }

        var scale = scaleMode switch
        {
            VncScaleMode.Fit => System.Math.Min(imageSize.Width / framebufferWidth, imageSize.Height / framebufferHeight),
            VncScaleMode.Fill => System.Math.Max(imageSize.Width / framebufferWidth, imageSize.Height / framebufferHeight),
            _ => 1d,
        };

        if (scale <= 0 || double.IsNaN(scale) || double.IsInfinity(scale))
        {
            return (-1, -1);
        }

        // Fit and Fill center the scaled screen in the image.
        var offsetX = scaleMode == VncScaleMode.Original ? 0 : (imageSize.Width - (framebufferWidth * scale)) / 2;
        var offsetY = scaleMode == VncScaleMode.Original ? 0 : (imageSize.Height - (framebufferHeight * scale)) / 2;

        var x = (int)((position.X - offsetX) / scale);
        var y = (int)((position.Y - offsetY) / scale);

        return (System.Math.Clamp(x, 0, framebufferWidth - 1), System.Math.Clamp(y, 0, framebufferHeight - 1));
    }
}