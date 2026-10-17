namespace Atc.Wpf.Tests.Controls.Media.W3cSvg;

/// <summary>
/// Renders SVG icons used by the library and the sample app to bitmaps and compares them with reference PNGs
/// in <c>RenderReferences</c>. The SVG viewBox is mapped onto the bitmap as it is (the drawing is not fitted to
/// its bounds), so a misplaced, mirrored or wrongly composed transform shows up as a changed image.
/// </summary>
/// <remarks>
/// When a reference PNG is missing the rendered image is written in its place and the test fails;
/// review the new PNG and commit it.
/// </remarks>
public sealed class SvgRenderSnapshotTests
{
    private const int ImageSize = 128;
    private const double Dpi = 96;
    private const int ChannelTolerance = 16;
    private const double MaxDifferingPixelRatio = 0.01;

    private const string ReferencesFolder = "test/Atc.Wpf.Tests/Controls/Media/W3cSvg/RenderReferences";

    public static TheoryData<string, string> Icons => new()
    {
        // translate(0,512) scale(0.1,-0.1): a vertical flip of the whole icon
        { "src/Atc.Wpf.Controls/Resources/Icons/collapse.svg", "collapse" },
        { "src/Atc.Wpf.Controls/Resources/Icons/expand.svg", "expand" },
        { "src/Atc.Wpf.Controls/Resources/Icons/question-mark.svg", "question-mark" },
        { "src/Atc.Wpf.Forms/Resources/LogCategoryIcons/information.svg", "information" },

        // Sample app assets: nested groups, CSS classes, clip-path
        { "sample/Atc.Wpf.Sample/Assets/a.svg", "a" },
        { "sample/Atc.Wpf.Sample/Assets/shout.svg", "shout" },
        { "sample/Atc.Wpf.Sample/Assets/eggeaster.svg", "eggeaster" },

        // Synthetic: rotate+translate, scale, mirror, nested groups, rotate about a point, skewX, matrix
        { $"{ReferencesFolder}/nested-groups.svg", "nested-groups" },
        { $"{ReferencesFolder}/transform-lists.svg", "transform-lists" },
    };

    [StaTheory]
    [MemberData(nameof(Icons))]
    public void Icon_RendersAsTheReferenceImage(
        string svgPath,
        string name)
    {
        var root = FindRepositoryRoot();
        var actual = RenderSvgFile(Path.Combine(root, svgPath));

        var referencePath = Path.GetFullPath(Path.Combine(root, ReferencesFolder, $"{name}.png"));
        if (!File.Exists(referencePath))
        {
            SavePng(actual, referencePath);
            Assert.Fail($"Reference created at '{referencePath}', review and commit it.");
        }

        var expected = LoadPng(referencePath);
        var (differing, total) = CountDifferingPixels(expected, actual);
        var ratio = (double)differing / total;

        if (ratio > MaxDifferingPixelRatio)
        {
            var actualPath = Path.ChangeExtension(referencePath, ".actual.png");
            SavePng(actual, actualPath);
            Assert.Fail(
                $"'{name}' differs from its reference in {differing} of {total} pixels ({ratio:P2}, allowed {MaxDifferingPixelRatio:P2}). " +
                $"Rendered image written to '{actualPath}'.");
        }
    }

    [StaTheory]
    [InlineData("translate(50,0) rotate(90)", 145, 110)]
    [InlineData("rotate(90) translate(50,0)", 95, 160)]
    public void TransformList_PlacesTheRenderedRectWhereSvgOrderPutsIt(
        string transform,
        double expectedCentroidX,
        double expectedCentroidY)
    {
        // A 20x10 rect at the origin, viewBox -100 -100 200 200 rendered 1:1, so pixel = user unit + 100.
        // translate(50,0) rotate(90): rotate to x[-10,0] y[0,20], then translate to x[40,50] y[0,20] -> centroid (45,10).
        // rotate(90) translate(50,0): translate to x[50,70] y[0,10], then rotate to x[-10,0] y[50,70] -> centroid (-5,60).
        var svg = $"""<svg viewBox="-100 -100 200 200" xmlns="http://www.w3.org/2000/svg"><rect x="0" y="0" width="20" height="10" fill="#000000" transform="{transform}" /></svg>""";

        var bitmap = Render(new SvgRender(), r => r.LoadXmlDrawing(svg), 200);
        var (x, y, count) = FilledCentroid(bitmap);

        Assert.InRange(count, 180, 220);
        Assert.InRange(x, expectedCentroidX - 1, expectedCentroidX + 1);
        Assert.InRange(y, expectedCentroidY - 1, expectedCentroidY + 1);
    }

    private static BitmapSource RenderSvgFile(string svgFile)
        => Render(new SvgRender(), r => r.LoadDrawing(svgFile), ImageSize);

    private static BitmapSource Render(
        SvgRender render,
        Func<SvgRender, DrawingGroup> load,
        int size)
    {
        var drawing = load(render);
        var svg = render.Svg ?? throw new InvalidOperationException("The SVG was not loaded.");
        var viewBox = svg.ViewBox ?? new Rect(0, 0, svg.Size.Width, svg.Size.Height);
        Assert.True(viewBox.Width > 0 && viewBox.Height > 0, "The SVG has neither a viewBox nor a size.");

        var scale = System.Math.Min(size / viewBox.Width, size / viewBox.Height);
        var offsetX = ((size - (viewBox.Width * scale)) / 2) - (viewBox.X * scale);
        var offsetY = ((size - (viewBox.Height * scale)) / 2) - (viewBox.Y * scale);

        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            dc.DrawRectangle(Brushes.White, pen: null, new Rect(0, 0, size, size));
            dc.PushTransform(new MatrixTransform(scale, 0, 0, scale, offsetX, offsetY));
            dc.DrawDrawing(drawing);
            dc.Pop();
        }

        var bitmap = new RenderTargetBitmap(size, size, Dpi, Dpi, PixelFormats.Pbgra32);
        bitmap.Render(visual);
        bitmap.Freeze();
        return bitmap;
    }

    private static (double X, double Y, int Count) FilledCentroid(
        BitmapSource bitmap)
    {
        var pixels = GetPixels(bitmap);
        var width = bitmap.PixelWidth;
        double sumX = 0;
        double sumY = 0;
        double weight = 0;
        var count = 0;

        for (var i = 0; i < pixels.Length; i += 4)
        {
            // Darkness of the pixel on the white background: 1 for a fully covered pixel, fractional on edges
            var coverage = (255 - pixels[i + 2]) / 255.0;
            if (coverage <= 0)
            {
                continue;
            }

            var index = i / 4;
            sumX += ((index % width) + 0.5) * coverage;
            sumY += ((index / width) + 0.5) * coverage;
            weight += coverage;
            if (coverage > 0.5)
            {
                count++;
            }
        }

        Assert.True(weight > 0, "Nothing was rendered inside the viewBox.");
        return (sumX / weight, sumY / weight, count);
    }

    private static (int Differing, int Total) CountDifferingPixels(
        BitmapSource expected,
        BitmapSource actual)
    {
        Assert.Equal(expected.PixelWidth, actual.PixelWidth);
        Assert.Equal(expected.PixelHeight, actual.PixelHeight);

        var expectedPixels = GetPixels(expected);
        var actualPixels = GetPixels(actual);
        var differing = 0;

        for (var i = 0; i < expectedPixels.Length; i += 4)
        {
            for (var channel = 0; channel < 4; channel++)
            {
                if (System.Math.Abs(expectedPixels[i + channel] - actualPixels[i + channel]) > ChannelTolerance)
                {
                    differing++;
                    break;
                }
            }
        }

        return (differing, expectedPixels.Length / 4);
    }

    private static byte[] GetPixels(BitmapSource bitmap)
    {
        var converted = bitmap.Format == PixelFormats.Pbgra32
            ? bitmap
            : new FormatConvertedBitmap(bitmap, PixelFormats.Pbgra32, destinationPalette: null, alphaThreshold: 0);
        var stride = converted.PixelWidth * 4;
        var pixels = new byte[stride * converted.PixelHeight];
        converted.CopyPixels(pixels, stride, 0);
        return pixels;
    }

    private static BitmapSource LoadPng(string path)
    {
        using var stream = File.OpenRead(path);
        var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        return decoder.Frames[0];
    }

    private static void SavePng(
        BitmapSource bitmap,
        string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = File.Create(path);
        encoder.Save(stream);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Atc.Wpf.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
               ?? throw new InvalidOperationException($"Could not find Atc.Wpf.slnx above '{AppContext.BaseDirectory}'.");
    }
}