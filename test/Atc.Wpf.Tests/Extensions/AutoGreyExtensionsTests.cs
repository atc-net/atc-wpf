namespace Atc.Wpf.Tests.Extensions;

public sealed class AutoGreyExtensionsTests : IDisposable
{
    private readonly string pngPath = Path.Combine(Path.GetTempPath(), "atc-autogrey-" + Guid.NewGuid().ToString("N") + ".png");

    public AutoGreyExtensionsTests()
    {
        // A red 2x2 image, so a grey version is easy to tell apart, with a transparent last pixel.
        var pixels = new byte[2 * 2 * 4];
        for (var i = 0; i < pixels.Length - 4; i += 4)
        {
            pixels[i + 2] = 255;
            pixels[i + 3] = 255;
        }

        var source = BitmapSource.Create(2, 2, 96, 96, PixelFormats.Bgra32, palette: null, pixels, stride: 8);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(source));
        using var stream = File.Create(pngPath);
        encoder.Save(stream);
    }

    public void Dispose()
    {
        File.Delete(pngPath);
        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void BitmapImage_AutoGreyEnabled_ReturnsTheSameImage()
    {
        var original = LoadFromFile();

        var result = original.AutoGrey(isEnabled: true);

        Assert.Same(original, result);
    }

    [StaFact]
    public void BitmapImage_AutoGreyDisabled_ReturnsAGreyImage()
    {
        var original = LoadFromFile();

        var result = original.AutoGrey(isEnabled: false);

        var color = GetBgraPixel(result, 0, 0);
        Assert.Equal(255, color.A);
        Assert.Equal(color.R, color.G);
        Assert.Equal(color.G, color.B);
        Assert.NotEqual(Colors.Red, color);
    }

    [StaFact]
    public void BitmapImage_AutoGreyDisabled_KeepsTransparency()
    {
        var original = LoadFromFile();

        var result = original.AutoGrey(isEnabled: false);

        Assert.Equal(0, GetBgraPixel(result, 1, 1).A);
    }

    [StaFact]
    public void Image_AutoGreyDisabled_ShowsAGreyVersionWithAnOpacityMask()
    {
        var image = new Image { Source = LoadFromFile() };

        image.AutoGrey(isEnabled: false);

        Assert.IsType<FormatConvertedBitmap>(image.Source);
        Assert.IsType<ImageBrush>(image.OpacityMask);
    }

    [StaFact]
    public void Image_AutoGreyDisabledThenEnabled_RestoresTheOriginalSource()
    {
        var original = LoadFromFile();
        var image = new Image { Source = original };

        image.AutoGrey(isEnabled: false);
        image.AutoGrey(isEnabled: true);

        Assert.Same(original, image.Source);
        Assert.Null(image.OpacityMask);
    }

    [StaFact]
    public void Image_AutoGreyDisabledTwice_KeepsTheOriginalToRestore()
    {
        var original = LoadFromFile();
        var image = new Image { Source = original };

        image.AutoGrey(isEnabled: false);
        image.AutoGrey(isEnabled: false);
        image.AutoGrey(isEnabled: true);

        Assert.Same(original, image.Source);
    }

    [StaFact]
    public void Image_AutoGreyEnabledWhileEnabled_KeepsTheSource()
    {
        var original = LoadFromFile();
        var image = new Image { Source = original };

        image.AutoGrey(isEnabled: true);

        Assert.Same(original, image.Source);
    }

    private BitmapImage LoadFromFile()
    {
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.UriSource = new Uri(pngPath);
        bitmapImage.EndInit();
        return bitmapImage;
    }

    private static Color GetBgraPixel(
        BitmapSource bitmap,
        int x,
        int y)
    {
        var bgra = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, destinationPalette: null, alphaThreshold: 0);
        var pixel = new byte[4];
        bgra.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);
        return Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);
    }
}