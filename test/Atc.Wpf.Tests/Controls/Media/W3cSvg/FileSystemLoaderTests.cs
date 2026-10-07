namespace Atc.Wpf.Tests.Controls.Media.W3cSvg;

public sealed class FileSystemLoaderTests : IDisposable
{
    private readonly string rootDirectory;
    private readonly string svgDirectory;
    private readonly string svgFilename;
    private readonly string outsideFile;

    public FileSystemLoaderTests()
    {
        rootDirectory = Path.Combine(Path.GetTempPath(), "atc-svg-loader-" + Guid.NewGuid().ToString("N"));
        svgDirectory = Path.Combine(rootDirectory, "svg");
        Directory.CreateDirectory(Path.Combine(svgDirectory, "images"));

        svgFilename = Path.Combine(svgDirectory, "drawing.svg");
        File.WriteAllText(svgFilename, "<svg />");
        File.WriteAllText(Path.Combine(svgDirectory, "local.png"), "local");
        File.WriteAllText(Path.Combine(svgDirectory, "images", "nested.png"), "nested");

        outsideFile = Path.Combine(rootDirectory, "secret.txt");
        File.WriteAllText(outsideFile, "secret");
    }

    [Theory]
    [InlineData("local.png", "local")]
    [InlineData("images/nested.png", "nested")]
    [InlineData("images\\nested.png", "nested")]
    public void LoadFile_RelativeHrefInsideSvgDirectory_ReturnsStream(
        string hRef,
        string expectedContent)
    {
        using var stream = FileSystemLoader.Instance.LoadFile(hRef, svgFilename);

        Assert.NotNull(stream);
        using var reader = new StreamReader(stream);
        Assert.Equal(expectedContent, reader.ReadToEnd());
    }

    [Fact]
    public void LoadFile_SvgAtDriveRoot_ResolvesRelativeHref()
    {
        var systemRoot = Path.GetPathRoot(Environment.SystemDirectory)!;
        var relativeHosts = Path.GetRelativePath(systemRoot, Path.Combine(Environment.SystemDirectory, "drivers", "etc", "hosts"));

        using var stream = FileSystemLoader.Instance.LoadFile(relativeHosts, Path.Combine(systemRoot, "drawing.svg"));

        Assert.NotNull(stream);
    }

    [Fact]
    public void LoadFile_AbsolutePathHref_ReturnsNull()
    {
        using var stream = FileSystemLoader.Instance.LoadFile(outsideFile, svgFilename);

        Assert.Null(stream);
    }

    [Fact]
    public void LoadFile_DevicePathHref_ReturnsNull()
    {
        // \\?\C:\... is rooted the same way a \\server\share UNC path is.
        using var stream = FileSystemLoader.Instance.LoadFile(@"\\?\" + outsideFile, svgFilename);

        Assert.Null(stream);
    }

    [Theory]
    [InlineData("../secret.txt")]
    [InlineData("..\\secret.txt")]
    [InlineData("images/../../secret.txt")]
    public void LoadFile_HrefEscapingSvgDirectory_ReturnsNull(string hRef)
    {
        using var stream = FileSystemLoader.Instance.LoadFile(hRef, svgFilename);

        Assert.Null(stream);
    }

    public void Dispose()
    {
        if (Directory.Exists(rootDirectory))
        {
            Directory.Delete(rootDirectory, recursive: true);
        }
    }
}