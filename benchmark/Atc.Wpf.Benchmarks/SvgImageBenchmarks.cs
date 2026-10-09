namespace Atc.Wpf.Benchmarks;

/// <summary>
/// Measures loading an <see cref="SvgImage"/>. <c>Load_like_xaml</c> sets the source before the control is
/// initialized, as XAML does, so a repeated icon is served from the drawing cache (an icon in an item template,
/// per row). <c>Load_cold</c> passes a stream after initialization, which always parses and renders.
/// </summary>
[MemoryDiagnoser]
public class SvgImageBenchmarks
{
    private const string FileIconSvg = """
        <svg viewBox="0 0 24 24">
          <g>
            <path d="M19 9V17.8C19 18.9201 19 19.4802 18.782 19.908C18.5903 20.2843 18.2843 20.5903 17.908 20.782C17.4802
                  21 16.9201 21 15.8 21H8.2C7.07989 21 6.51984 21 6.09202 20.782C5.71569 20.5903 5.40973 20.2843 5.21799
                  19.908C5 19.4802 5 18.9201 5 17.8V6.2C5 5.07989 5 4.51984 5.21799 4.09202C5.40973 3.71569 5.71569 3.40973
                  6.09202 3.21799C6.51984 3 7.0799 3 8.2 3H13M19 9L13 3M19 9H14C13.4477 9 13 8.55228 13 8V3"
                  stroke="#000000" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" fill="none" />
          </g>
        </svg>
        """;

    private string svgFile = string.Empty;
    private byte[] svgBytes = [];

    [GlobalSetup]
    public void Setup()
    {
        svgFile = Path.Combine(Path.GetTempPath(), $"atc-svg-benchmark-{Guid.NewGuid():N}.svg");
        File.WriteAllText(svgFile, FileIconSvg);
        svgBytes = File.ReadAllBytes(svgFile);
    }

    [GlobalCleanup]
    public void Cleanup()
        => File.Delete(svgFile);

    [Benchmark(Baseline = true)]
    [STAThread]
    public SvgImage Load_cold()
    {
        var svgImage = new SvgImage();
        svgImage.BeginInit();
        svgImage.EndInit();
        using var stream = new MemoryStream(svgBytes, writable: false);
        svgImage.SetImage(stream);
        return svgImage;
    }

    [Benchmark]
    [STAThread]
    public SvgImage Load_like_xaml()
    {
        var svgImage = new SvgImage();
        svgImage.BeginInit();
        svgImage.FileSource = svgFile;
        svgImage.EndInit();
        return svgImage;
    }

    [Benchmark]
    [STAThread]
    public SvgImage Load_like_xaml_with_override_color()
    {
        var svgImage = new SvgImage();
        svgImage.BeginInit();
        svgImage.OverrideColor = Colors.SteelBlue;
        svgImage.FileSource = svgFile;
        svgImage.EndInit();
        return svgImage;
    }
}