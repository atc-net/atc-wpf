namespace Atc.Wpf.Helpers;

public static class BitmapImageHelper
{
    private const string Base64Header = "base64,";

    public static BitmapImage? ConvertFromBase64(
        string base64Value)
    {
        if (string.IsNullOrWhiteSpace(base64Value))
        {
            return null;
        }

        var index = base64Value.IndexOf(Base64Header, StringComparison.OrdinalIgnoreCase);
        if (index > -1)
        {
            base64Value = base64Value[(index + Base64Header.Length)..];
        }

        var imageBytes = Convert.FromBase64String(base64Value);

        var bitmap = new BitmapImage();
        try
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                ms.Position = 0;
                bitmap.BeginInit();
                bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = null;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
            }

            bitmap.Freeze();
            return bitmap;
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }

    public static string ConvertToBase64DataImage(
        BitmapImage image,
        ImageFormatType imageFormatType)
        => image.ToBase64DataImage(imageFormatType);
}