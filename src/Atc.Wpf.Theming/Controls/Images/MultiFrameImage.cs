namespace Atc.Wpf.Theming.Controls.Images;

public class MultiFrameImage : Image
{
    public static readonly DependencyProperty MultiFrameImageModeProperty = DependencyProperty.Register(
        nameof(MultiFrameImageMode),
        typeof(MultiFrameImageMode),
        typeof(MultiFrameImage),
        new FrameworkPropertyMetadata(
            MultiFrameImageMode.ScaleDownLargerFrame,
            FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets the mode for the rendered image.
    /// </summary>
    public MultiFrameImageMode MultiFrameImageMode
    {
        get => (MultiFrameImageMode)GetValue(MultiFrameImageModeProperty);
        set => SetValue(MultiFrameImageModeProperty, value);
    }

    static MultiFrameImage()
    {
        SourceProperty.OverrideMetadata(typeof(MultiFrameImage), new FrameworkPropertyMetadata(OnSourceChanged));
    }

    private static void OnSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var multiFrameImage = (MultiFrameImage)d;
        multiFrameImage.UpdateFrameList();
    }

    private readonly List<BitmapSource> frames = new();

    private void UpdateFrameList()
    {
        frames.Clear();

        var decoder = (Source as BitmapFrame)?.Decoder;
        if (decoder is null || decoder.Frames.Count == 0)
        {
            return;
        }

        frames.AddRange(decoder
            .Frames
            .GroupBy(f => f.PixelWidth * f.PixelHeight)
            .OrderBy(g => g.Key)
            .Select(g => g.OrderByDescending(f => f.Format.BitsPerPixel).First()));
    }

    protected override void OnRender(DrawingContext dc)
    {
        ArgumentNullException.ThrowIfNull(dc);

        if (frames.Count == 0)
        {
            base.OnRender(dc);
            return;
        }

        switch (MultiFrameImageMode)
        {
            case MultiFrameImageMode.ScaleDownLargerFrame:
                var minSize = System.Math.Max(RenderSize.Width, RenderSize.Height);
                var minFrame = frames.Find(f => f.Width >= minSize && f.Height >= minSize) ?? frames.Last();
                dc.DrawImage(minFrame, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
                break;
            case MultiFrameImageMode.NoScaleSmallerFrame:
                var maxSize = System.Math.Min(RenderSize.Width, RenderSize.Height);
                var maxFrame = frames.LastOrDefault(f => f.Width <= maxSize && f.Height <= maxSize) ?? frames[0];
                dc.DrawImage(maxFrame, new Rect((RenderSize.Width - maxFrame.Width) / 2, (RenderSize.Height - maxFrame.Height) / 2, maxFrame.Width, maxFrame.Height));
                break;
            default:
                throw new SwitchCaseDefaultException(MultiFrameImageMode);
        }
    }
}