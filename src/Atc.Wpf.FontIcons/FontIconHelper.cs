namespace Atc.Wpf.FontIcons;

internal static class FontIconHelper
{
    /// <summary>
    /// The em-size the shared icon images are laid out with; the same size the bitmap images used.
    /// </summary>
    private const double IconEmSize = 100;

    /// <summary>
    /// The number of shared icon images kept before the cache is cleared.
    /// </summary>
    private const int MaxCachedIconImages = 4096;

    private static readonly ConcurrentDictionary<IconImageKey, DrawingImage> IconImageCache = new();
    internal static object SpinDurationCoerceValue(
        DependencyObject d,
        object value)
    {
        var val = (double)value;
        return val < 0 ? 0d : value;
    }

    [SuppressMessage("Major Code Smell", "S3358:Ternary operators should not be nested", Justification = "OK.")]
    internal static object RotationCoerceValue(
        DependencyObject d,
        object value)
    {
        var val = (double)value;
        return val < 0
            ? 0d
            : val > 360
                ? 360d
                : value;
    }

    internal static ImageSource CreateImageSource(
        Typeface typeface,
        string iconChar,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var formattedText = new FormattedText(
            iconChar,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            typeface,
            emSize,
            foregroundBrush,
            pixelsPerDip: 1)
        {
            TextAlignment = TextAlignment.Center,
        };

        var width = (int)formattedText.Width > 0
            ? (int)formattedText.Width
            : (int)(emSize * iconChar.Length);

        var widthHalf = width / 2;
        var height = (int)formattedText.Height;

        var drawingVisual = new DrawingVisual();
        using (var drawingContext = drawingVisual.RenderOpen())
        {
            drawingContext.DrawText(formattedText, new Point(widthHalf, 0));
        }

        var renderTargetBitmap = new RenderTargetBitmap(
            width,
            height,
            96,
            96,
            PixelFormats.Pbgra32);

        renderTargetBitmap.Render(drawingVisual);
        renderTargetBitmap.Freeze();
        return renderTargetBitmap;
    }

    internal static DrawingImage CreateDrawingImage(
        Typeface typeface,
        string iconChar,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var formattedText = new FormattedText(
            iconChar,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            typeface,
            emSize,
            foregroundBrush,
            pixelsPerDip: 1)
        {
            TextAlignment = TextAlignment.Center,
        };

        var visual = new DrawingVisual();
        using (var drawingContext = visual.RenderOpen())
        {
            drawingContext.DrawText(formattedText, new Point(0, 0));
        }

        var drawingImage = new DrawingImage(visual.Drawing);
        drawingImage.Freeze();
        return drawingImage;
    }

    /// <summary>
    /// Gets a frozen vector image of an icon glyph for an Image* icon control.
    /// </summary>
    /// <remarks>
    /// The image has the same size and glyph position as <see cref="CreateImageSource"/> (a line-high box at
    /// <see cref="IconEmSize"/>), but scales without blurring and is shared by every icon with the same glyph
    /// and solid color, so a theme switch only looks up an image instead of rasterizing one per icon.
    /// </remarks>
    internal static ImageSource GetIconImageSource(
        Typeface typeface,
        string iconChar,
        Brush? foregroundBrush)
    {
        if (foregroundBrush is not SolidColorBrush solidColorBrush)
        {
            return CreateIconDrawingImage(typeface, iconChar, foregroundBrush);
        }

        var key = new IconImageKey(typeface, iconChar, solidColorBrush.Color, solidColorBrush.Opacity);
        if (IconImageCache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        if (IconImageCache.Count >= MaxCachedIconImages)
        {
            IconImageCache.Clear();
        }

        var frozenBrush = new SolidColorBrush(solidColorBrush.Color)
        {
            Opacity = solidColorBrush.Opacity,
        };
        frozenBrush.Freeze();

        return IconImageCache.GetOrAdd(key, CreateIconDrawingImage(typeface, iconChar, frozenBrush));
    }

    private static DrawingImage CreateIconDrawingImage(
        Typeface typeface,
        string iconChar,
        Brush? foregroundBrush)
    {
        var formattedText = new FormattedText(
            iconChar,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            typeface,
            IconEmSize,
            foregroundBrush,
            pixelsPerDip: 1)
        {
            TextAlignment = TextAlignment.Center,
        };

        var width = (int)formattedText.Width > 0
            ? (int)formattedText.Width
            : (int)(IconEmSize * iconChar.Length);
        var height = (int)formattedText.Height;
        var bounds = new Rect(0, 0, width, height);

        var drawingVisual = new DrawingVisual();
        using (var drawingContext = drawingVisual.RenderOpen())
        {
            // A transparent line-high box keeps the image size, and the clip keeps the glyph within it,
            // as the bitmap image did.
            drawingContext.DrawRectangle(Brushes.Transparent, pen: null, bounds);
            drawingContext.PushClip(new RectangleGeometry(bounds));
            drawingContext.DrawText(formattedText, new Point(width / 2, 0));
            drawingContext.Pop();
        }

        var drawingImage = new DrawingImage(drawingVisual.Drawing);
        if (drawingImage.CanFreeze)
        {
            drawingImage.Freeze();
        }

        return drawingImage;
    }

    private readonly record struct IconImageKey(
        Typeface Typeface,
        string IconChar,
        Color Color,
        double Opacity);
}