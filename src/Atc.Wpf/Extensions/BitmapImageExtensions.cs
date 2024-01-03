namespace Atc.Wpf.Extensions;

public static class BitmapImageExtensions
{
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