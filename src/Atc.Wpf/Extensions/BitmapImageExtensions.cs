namespace Atc.Wpf.Extensions;

/// <summary>
/// Extension methods for <see cref="BitmapImage"/>.
/// </summary>
public static class BitmapImageExtensions
{
    /// <summary>
    /// Returns the image unchanged when enabled; otherwise attempts to return a grayscale version of it.
    /// </summary>
    public static BitmapImage AutoGrey(
        this BitmapImage image,
        bool isEnabled)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (isEnabled)
        {
            return image;
        }

        var grayBitmapSource = new FormatConvertedBitmap();
        grayBitmapSource.BeginInit();
        grayBitmapSource.Source = image;
        grayBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
        grayBitmapSource.EndInit();

        var grayImage = new Image
        {
            Source = grayBitmapSource,
        };

        return (BitmapImage)grayImage.Source;
    }
}