namespace Atc.Wpf.Hardware.Tests.Pickers.Internal;

public sealed class CameraPreviewFormatTests
{
    [Fact]
    public void GetReaderSize_FullHdShownAt240Pixels_ScalesDownKeepingTheAspectRatio()
    {
        var size = CameraPreviewFormat.GetReaderSize(1920, 1080, 240);

        Assert.Equal((426u, 240u), size);
    }

    [Fact]
    public void GetReaderSize_OddPixelHeight_RoundsToEvenDimensions()
    {
        var size = CameraPreviewFormat.GetReaderSize(1280, 720, 300.4);

        Assert.NotNull(size);
        Assert.Equal(0u, size.Value.Width % 2);
        Assert.Equal(0u, size.Value.Height % 2);
        Assert.Equal(302u, size.Value.Height);
    }

    [Theory]
    [InlineData(640u, 480u, 480)]
    [InlineData(640u, 480u, 1000)]
    [InlineData(0u, 0u, 240)]
    [InlineData(1920u, 1080u, 0)]
    [InlineData(1920u, 1080u, double.NaN)]
    public void GetReaderSize_NoReductionPossible_KeepsTheCameraSize(
        uint width,
        uint height,
        double previewPixelHeight)
        => Assert.Null(CameraPreviewFormat.GetReaderSize(width, height, previewPixelHeight));

    [Fact]
    public void ToWpfPixelFormat_FollowsTheAlphaMode()
    {
        Assert.Equal(System.Windows.Media.PixelFormats.Pbgra32, CameraPreviewFormat.ToWpfPixelFormat(Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied));
        Assert.Equal(System.Windows.Media.PixelFormats.Bgra32, CameraPreviewFormat.ToWpfPixelFormat(Windows.Graphics.Imaging.BitmapAlphaMode.Straight));

        // Ignore-alpha frames can carry alpha 0; an alpha-less format keeps them visible.
        Assert.Equal(System.Windows.Media.PixelFormats.Bgr32, CameraPreviewFormat.ToWpfPixelFormat(Windows.Graphics.Imaging.BitmapAlphaMode.Ignore));
    }

    [Fact]
    public void CopyRows_DifferentStrides_CopiesOnlyThePixelBytesOfEachRow()
    {
        byte[] source = [1, 2, 0, 3, 4, 0];
        var destination = new byte[8];

        var copied = CameraPreviewFormat.CopyRows(source, sourceStride: 3, destination, destinationStride: 4, rowBytes: 2, rows: 2);

        Assert.True(copied);
        Assert.Equal([1, 2, 0, 0, 3, 4, 0, 0], destination);
    }

    [Fact]
    public void CopyRows_PackedRows_CopiesTheWholeBlock()
    {
        byte[] source = [1, 2, 3, 4];
        var destination = new byte[4];

        Assert.True(CameraPreviewFormat.CopyRows(source, 2, destination, 2, 2, 2));
        Assert.Equal(source, destination);
    }

    [Fact]
    public void CopyRows_SourceTooSmall_CopiesNothing()
    {
        byte[] source = [1, 2, 3];
        var destination = new byte[4];

        Assert.False(CameraPreviewFormat.CopyRows(source, 2, destination, 2, 2, 2));
        Assert.Equal(new byte[4], destination);
    }
}