namespace Atc.Wpf.Tests.Controls.Media.W3cSvg;

public sealed class SvgImageElementTests : IDisposable
{
    private const string SvgOpen = """<svg viewBox="0 0 10 10" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">""";

    private readonly string directory;

    public SvgImageElementTests()
    {
        directory = Path.Combine(Path.GetTempPath(), $"atc-svg-image-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
    }

    public void Dispose()
        => Directory.Delete(directory, recursive: true);

    [StaTheory]
    [InlineData("image/png", "xlink:href")]
    [InlineData("image/gif", "xlink:href")]
    [InlineData("image/bmp", "href")]
    [InlineData("image/png", "href")]
    public void EmbeddedImage_AnyImageTypeAndEitherHref_IsLoaded(
        string mimeType,
        string hrefAttribute)
    {
        var base64 = Convert.ToBase64String(EncodeImage(mimeType));

        var drawing = new SvgRender().LoadXmlDrawing(
            $"""{SvgOpen}<image width="2" height="2" {hrefAttribute}="data:{mimeType};base64,{base64}" /></svg>""");

        var imageDrawing = Assert.Single(ImageDrawings(drawing));
        var bitmap = Assert.IsAssignableFrom<BitmapSource>(imageDrawing.ImageSource);
        Assert.Equal(2, bitmap.PixelWidth);
        Assert.True(bitmap.IsFrozen);
    }

    [StaFact]
    public void ExternalImage_AfterLoading_IsNotLockedAndIsFrozen()
    {
        var pngFile = Path.Combine(directory, "picture.png");
        File.WriteAllBytes(pngFile, EncodeImage("image/png"));
        var svgFile = Path.Combine(directory, "drawing.svg");
        File.WriteAllText(svgFile, $"""{SvgOpen}<image width="2" height="2" href="picture.png" /></svg>""");

        var drawing = new SvgRender().LoadDrawing(svgFile);

        var bitmap = Assert.IsAssignableFrom<BitmapSource>(Assert.Single(ImageDrawings(drawing)).ImageSource);
        Assert.Equal(2, bitmap.PixelWidth);
        Assert.True(bitmap.IsFrozen);
        using var exclusive = new FileStream(pngFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.True(exclusive.CanWrite);
    }

    private static byte[] EncodeImage(string mimeType)
    {
        var pixels = new byte[2 * 2 * 4];
        Array.Fill(pixels, (byte)0xFF);
        var source = BitmapSource.Create(2, 2, 96, 96, PixelFormats.Bgra32, palette: null, pixels, stride: 8);
        BitmapEncoder encoder = mimeType switch
        {
            "image/gif" => new GifBitmapEncoder(),
            "image/bmp" => new BmpBitmapEncoder(),
            _ => new PngBitmapEncoder(),
        };

        encoder.Frames.Add(BitmapFrame.Create(source));
        using var stream = new MemoryStream();
        encoder.Save(stream);
        return stream.ToArray();
    }

    private static List<ImageDrawing> ImageDrawings(DrawingGroup group)
    {
        var result = new List<ImageDrawing>();
        foreach (var child in group.Children)
        {
            switch (child)
            {
                case ImageDrawing imageDrawing:
                    result.Add(imageDrawing);
                    break;
                case DrawingGroup childGroup:
                    result.AddRange(ImageDrawings(childGroup));
                    break;
            }
        }

        return result;
    }
}