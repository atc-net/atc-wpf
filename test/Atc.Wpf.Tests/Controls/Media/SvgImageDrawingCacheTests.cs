namespace Atc.Wpf.Tests.Controls.Media;

public sealed class SvgImageDrawingCacheTests : IDisposable
{
    private const string BlueSquareSvg = """
        <svg viewBox="0 0 10 10" xmlns="http://www.w3.org/2000/svg">
          <rect width="10" height="10" fill="#0000FF" />
        </svg>
        """;

    private const string RedSquareSvg = """
        <svg viewBox="0 0 10 10" xmlns="http://www.w3.org/2000/svg">
          <rect width="10" height="10" fill="#FF0000" />
        </svg>
        """;

    private const string GradientSquareSvg = """
        <svg viewBox="0 0 10 10" xmlns="http://www.w3.org/2000/svg">
          <defs>
            <linearGradient id="fillBrush">
              <stop offset="0" stop-color="#0000FF" />
              <stop offset="1" stop-color="#0000FF" />
            </linearGradient>
          </defs>
          <rect width="10" height="10" fill="url(#fillBrush)" />
        </svg>
        """;

    private const string SpinningSquareSvg = """
        <svg viewBox="0 0 10 10" xmlns="http://www.w3.org/2000/svg">
          <g>
            <rect width="10" height="10" fill="#0000FF" />
            <animateTransform attributeName="transform" type="rotate" from="0" to="360" dur="1s" repeatCount="indefinite" />
          </g>
        </svg>
        """;

    private readonly string svgFile = Path.Combine(Path.GetTempPath(), $"atc-svgcache-{Guid.NewGuid():N}.svg");

    public void Dispose()
    {
        File.Delete(svgFile);
        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void SameSource_TwoImages_ShareOneFrozenDrawing()
    {
        File.WriteAllText(svgFile, BlueSquareSvg);

        var first = LoadLikeXaml();
        var second = LoadLikeXaml();

        Assert.Same(first.CurrentDrawing, second.CurrentDrawing);
        Assert.True(second.CurrentDrawing!.IsFrozen);
        Assert.True(second.IsDrawingFromCache);
        Assert.Equal(Colors.Blue, CenterColor(second));
    }

    [StaFact]
    public void SameSource_DifferentOverrideColor_DoesNotShareTheDrawing()
    {
        File.WriteAllText(svgFile, BlueSquareSvg);

        var blue = LoadLikeXaml();
        var red = LoadLikeXaml(svgImage => svgImage.OverrideColor = Colors.Red);

        Assert.NotSame(blue.CurrentDrawing, red.CurrentDrawing);
        Assert.Equal(Colors.Red, CenterColor(red));
    }

    [StaFact]
    public void CachedImage_OverrideColorChangedAfterLoad_RendersTheNewColor()
    {
        File.WriteAllText(svgFile, BlueSquareSvg);
        var first = LoadLikeXaml();
        var sut = LoadLikeXaml();

        sut.OverrideColor = Colors.Red;

        Assert.Equal(Colors.Red, CenterColor(sut));
        Assert.Equal(Colors.Blue, CenterColor(first));
    }

    // A theme switch changes the override on every icon: after the first icon, the rest reuse its drawing.
    [StaFact]
    public void OverrideColorChangedAfterLoad_DrawingForThatColorIsCached_IsShared()
    {
        File.WriteAllText(svgFile, BlueSquareSvg);
        var red = LoadLikeXaml(svgImage => svgImage.OverrideColor = Colors.Red);
        var sut = LoadLikeXaml();

        sut.OverrideColor = Colors.Red;

        Assert.Same(red.CurrentDrawing, sut.CurrentDrawing);
    }

    [StaFact]
    public void CachedImage_CustomBrushesSetAfterLoad_AreUsed()
    {
        File.WriteAllText(svgFile, GradientSquareSvg);
        _ = LoadLikeXaml();
        var sut = LoadLikeXaml();

        sut.CustomBrushes = new Dictionary<string, Brush>(StringComparer.Ordinal) { ["fillBrush"] = Brushes.Red };

        Assert.Equal(Colors.Red, CenterColor(sut));
    }

    [StaFact]
    public void CachedImage_CustomBrushEditedInPlaceThenReRendered_IsUsed()
    {
        File.WriteAllText(svgFile, GradientSquareSvg);
        _ = LoadLikeXaml();
        var sut = LoadLikeXaml();

        sut.CustomBrushes["fillBrush"] = Brushes.Red;
        sut.ReRenderSvg();

        Assert.Equal(Colors.Red, CenterColor(sut));
    }

    [StaFact]
    public void CachedImage_ExposesThePaintServersInCustomBrushes()
    {
        File.WriteAllText(svgFile, GradientSquareSvg);
        _ = LoadLikeXaml();

        var sut = LoadLikeXaml();

        Assert.True(sut.IsDrawingFromCache);
        var brush = Assert.IsAssignableFrom<Brush>(sut.CustomBrushes["fillBrush"]);
        Assert.False(brush.IsFrozen);
    }

    [StaFact]
    public void CustomBrushesSetBeforeLoad_AreNotTakenFromOrAddedToTheCache()
    {
        File.WriteAllText(svgFile, GradientSquareSvg);
        _ = LoadLikeXaml();

        var sut = LoadLikeXaml(svgImage => svgImage.CustomBrushes = new Dictionary<string, Brush>(StringComparer.Ordinal) { ["fillBrush"] = Brushes.Red });

        Assert.False(sut.IsDrawingFromCache);
        Assert.Equal(Colors.Red, CenterColor(sut));
        Assert.Equal(Colors.Blue, CenterColor(LoadLikeXaml()));
    }

    [StaFact]
    public void SameFileRewritten_LoadsTheNewContent()
    {
        File.WriteAllText(svgFile, BlueSquareSvg);
        _ = LoadLikeXaml();

        File.WriteAllText(svgFile, RedSquareSvg);
        File.SetLastWriteTimeUtc(svgFile, DateTime.UtcNow.AddMinutes(1));
        var sut = LoadLikeXaml();

        Assert.Equal(Colors.Red, CenterColor(sut));
    }

    [StaFact]
    public void AnimatedSvg_WithAnimations_IsNotShared()
    {
        File.WriteAllText(svgFile, SpinningSquareSvg);

        var first = LoadLikeXaml();
        var second = LoadLikeXaml();

        Assert.NotSame(first.CurrentDrawing, second.CurrentDrawing);
        Assert.False(second.IsDrawingFromCache);
    }

    // XAML sets the properties between BeginInit and EndInit, so the image loads in OnInitialized.
    private SvgImage LoadLikeXaml(Action<SvgImage>? configure = null)
    {
        var svgImage = new SvgImage { Width = 20, Height = 20 };
        svgImage.BeginInit();
        configure?.Invoke(svgImage);
        svgImage.FileSource = svgFile;
        svgImage.EndInit();
        return svgImage;
    }

    // Hosted in a window that is never shown, so the image renders as it does in an application.
    private static Color CenterColor(FrameworkElement element)
    {
        var root = new Border { Width = 20, Height = 20, Child = element };
        using var source = new HwndSource(new HwndSourceParameters("SvgImageDrawingCacheTests") { WindowStyle = 0 })
        {
            SizeToContent = SizeToContent.WidthAndHeight,
            RootVisual = root,
        };
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

        var bitmap = new RenderTargetBitmap(20, 20, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(root);
        root.Child = null;

        var pixel = new byte[4];
        bitmap.CopyPixels(new Int32Rect(10, 10, 1, 1), pixel, 4, 0);
        return Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);
    }
}