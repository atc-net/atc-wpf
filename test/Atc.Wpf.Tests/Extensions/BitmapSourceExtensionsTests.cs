namespace Atc.Wpf.Tests.Extensions;

public sealed class BitmapSourceExtensionsTests
{
    [StaFact]
    public void InvertColors_inverts_the_color_channels_and_keeps_the_alpha_of_a_Bgra32_bitmap()
    {
        // Two pixels, B G R A: an opaque one and a half transparent one.
        var source = CreateBitmap(PixelFormats.Bgra32, [10, 20, 30, 255, 100, 150, 200, 128]);

        var actual = source.InvertColors();

        Assert.Equal(PixelFormats.Bgra32, actual.Format);
        Assert.Equal(new byte[] { 245, 235, 225, 255, 155, 105, 55, 128 }, ReadPixels(actual));
    }

    [StaFact]
    public void InvertColors_keeps_an_opaque_bitmap_opaque()
    {
        var source = CreateBitmap(PixelFormats.Bgra32, [0, 0, 0, 255, 255, 255, 255, 255]);

        var pixels = ReadPixels(source.InvertColors());

        Assert.Equal(255, pixels[3]);
        Assert.Equal(255, pixels[7]);
    }

    [StaFact]
    public void InvertColors_handles_a_Bgr24_bitmap_by_adding_an_opaque_alpha()
    {
        var source = CreateBitmap(PixelFormats.Bgr24, [10, 20, 30, 100, 150, 200]);

        var actual = source.InvertColors();

        Assert.Equal(PixelFormats.Bgra32, actual.Format);
        Assert.Equal(2, actual.PixelWidth);
        Assert.Equal(new byte[] { 245, 235, 225, 255, 155, 105, 55, 255 }, ReadPixels(actual));
    }

    [StaFact]
    public void InvertColors_un_premultiplies_a_Pbgra32_bitmap_before_inverting()
    {
        // Half transparent red, premultiplied: B G R A = 0 0 128 128.
        var source = CreateBitmap(PixelFormats.Pbgra32, [0, 0, 128, 128, 0, 0, 0, 0]);

        var actual = source.InvertColors();

        Assert.Equal(PixelFormats.Bgra32, actual.Format);
        var pixels = ReadPixels(actual);
        Assert.Equal(255, pixels[0]);
        Assert.Equal(255, pixels[1]);
        Assert.Equal(0, pixels[2]);
        Assert.Equal(128, pixels[3]);
    }

    [Fact]
    public void InvertColors_throws_for_null()
        => Assert.Throws<ArgumentNullException>(() => BitmapSourceExtensions.InvertColors(bitmapSource: null!));

    private static BitmapSource CreateBitmap(
        PixelFormat format,
        byte[] pixels)
    {
        var stride = (2 * format.BitsPerPixel) / 8;
        var bitmap = BitmapSource.Create(2, 1, 96, 96, format, palette: null, pixels, stride);
        bitmap.Freeze();
        return bitmap;
    }

    private static byte[] ReadPixels(BitmapSource bitmap)
    {
        var stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel) / 8;
        var data = new byte[stride * bitmap.PixelHeight];
        bitmap.CopyPixels(data, stride, 0);
        return data;
    }
}