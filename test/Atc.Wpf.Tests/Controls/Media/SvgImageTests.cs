namespace Atc.Wpf.Tests.Controls.Media;

public sealed class SvgImageTests : IDisposable
{
    // A square filled through the paint server "fillBrush" (a blue gradient).
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

    private readonly string svgFile;

    public SvgImageTests()
    {
        svgFile = Path.Combine(Path.GetTempPath(), $"atc-svgimage-{Guid.NewGuid():N}.svg");
        File.WriteAllText(svgFile, GradientSquareSvg);
    }

    public void Dispose()
    {
        File.Delete(svgFile);
        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void LoadedLikeXaml_WithoutCustomBrushes_UsesThePaintServerFromTheSvg()
    {
        var sut = LoadLikeXaml(customBrushes: null);

        Assert.Equal(Colors.Blue, CenterColor(sut));
    }

    [StaFact]
    public void LoadedLikeXaml_CustomBrushForAPaintServer_IsUsedInsteadOfTheSvgBrush()
    {
        var sut = LoadLikeXaml(new Dictionary<string, Brush>(StringComparer.Ordinal) { ["fillBrush"] = Brushes.Red });

        Assert.Equal(Colors.Red, CenterColor(sut));
    }

    [StaFact]
    public void LoadedLikeXaml_CustomBrushForAPaintServer_IsKeptInCustomBrushes()
    {
        var sut = LoadLikeXaml(new Dictionary<string, Brush>(StringComparer.Ordinal) { ["fillBrush"] = Brushes.Red });

        Assert.Same(Brushes.Red, sut.CustomBrushes["fillBrush"]);
    }

    [StaFact]
    public void LoadedLikeXaml_WithoutCustomBrushes_ExposesTheSvgPaintServersInCustomBrushes()
    {
        var sut = LoadLikeXaml(customBrushes: null);

        Assert.True(sut.CustomBrushes.ContainsKey("fillBrush"));
    }

    [StaFact]
    public void LoadedLikeXaml_CustomBrushEditedInPlaceThenReRendered_IsUsed()
    {
        var sut = LoadLikeXaml(customBrushes: null);

        sut.CustomBrushes["fillBrush"] = Brushes.Red;
        sut.ReRenderSvg();

        Assert.Equal(Colors.Red, CenterColor(sut));
    }

    // XAML sets the properties between BeginInit and EndInit, so the image loads in OnInitialized.
    private SvgImage LoadLikeXaml(Dictionary<string, Brush>? customBrushes)
    {
        var svgImage = new SvgImage { Width = 20, Height = 20 };
        svgImage.BeginInit();
        if (customBrushes is not null)
        {
            svgImage.CustomBrushes = customBrushes;
        }

        svgImage.FileSource = svgFile;
        svgImage.EndInit();
        return svgImage;
    }

    // Hosted in a window that is never shown, so the image renders as it does in an application.
    private static Color CenterColor(FrameworkElement element)
    {
        var root = new Border { Width = 20, Height = 20, Child = element };
        using var source = new HwndSource(new HwndSourceParameters("SvgImageTests") { WindowStyle = 0 })
        {
            SizeToContent = SizeToContent.WidthAndHeight,
            RootVisual = root,
        };
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

        var bitmap = new RenderTargetBitmap(20, 20, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(root);

        var pixel = new byte[4];
        bitmap.CopyPixels(new Int32Rect(10, 10, 1, 1), pixel, 4, 0);
        return Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);
    }
}