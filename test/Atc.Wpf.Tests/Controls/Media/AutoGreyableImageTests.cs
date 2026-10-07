namespace Atc.Wpf.Tests.Controls.Media;

public sealed class AutoGreyableImageTests : IDisposable
{
    private readonly string pngPath = Path.Combine(Path.GetTempPath(), "atc-greyable-" + Guid.NewGuid().ToString("N") + ".png");

    public AutoGreyableImageTests()
    {
        var pixels = new byte[2 * 2 * 4];
        Array.Fill(pixels, (byte)200);
        var source = BitmapSource.Create(2, 2, 96, 96, PixelFormats.Bgra32, palette: null, pixels, stride: 8);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(source));
        using var stream = File.Create(pngPath);
        encoder.Save(stream);
    }

    [StaFact]
    public void Enable_RestoresTheOriginalSourceInstance()
    {
        var original = LoadFromFile();
        var image = new AutoGreyableImage { Source = original };

        image.IsEnabled = false;
        image.IsEnabled = true;

        Assert.Same(original, image.Source);
    }

    [StaFact]
    public void DisableTwice_ReusesTheGreyBitmap()
    {
        var image = new AutoGreyableImage { Source = LoadFromFile() };

        image.IsEnabled = false;
        var firstGrey = image.Source;
        image.IsEnabled = true;
        image.IsEnabled = false;

        Assert.IsType<FormatConvertedBitmap>(firstGrey);
        Assert.Same(firstGrey, image.Source);
    }

    [StaFact]
    public void ImagesSharingASource_ShareTheGreyBitmap()
    {
        var shared = LoadFromFile();
        var first = new AutoGreyableImage { Source = shared };
        var second = new AutoGreyableImage { Source = shared };

        first.IsEnabled = false;
        second.IsEnabled = false;

        Assert.Same(first.Source, second.Source);
    }

    [StaFact]
    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is how the leak is detected.")]
    public void GreyBitmap_IsNotKeptAliveAfterTheImageAndSourceAreGone()
    {
        var greyReference = DisableAndDropImage();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(greyReference.IsAlive);
    }

    public void Dispose()
    {
        if (File.Exists(pngPath))
        {
            File.Delete(pngPath);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private WeakReference DisableAndDropImage()
    {
        var image = new AutoGreyableImage { Source = LoadFromFile() };
        image.IsEnabled = false;
        return new WeakReference(image.Source);
    }

    private BitmapImage LoadFromFile()
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(pngPath);
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }
}