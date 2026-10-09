namespace Atc.Wpf.Helpers;

/// <summary>Provides helper methods for converting <see cref="BitmapImage"/> instances to and from Base64.</summary>
public static class BitmapImageHelper
{
    private const string Base64Header = "base64,";

    /// <summary>Creates a frozen <see cref="BitmapImage"/> from a Base64 string (an optional data URI prefix ending with <c>base64,</c> is ignored), or returns <see langword="null"/> if the value is empty or the image cannot be decoded.</summary>
    [SuppressMessage("X", "S1696:Do not catch NullReferenceException", Justification = "OK.")]
    public static BitmapImage? ConvertFromBase64(string base64Value)
    {
        if (string.IsNullOrWhiteSpace(base64Value))
        {
            return null;
        }

        var index = base64Value.IndexOf(
            Base64Header,
            StringComparison.OrdinalIgnoreCase);
        if (index > -1)
        {
            base64Value = base64Value[(index + Base64Header.Length)..];
        }

        var imageBytes = Convert.FromBase64String(base64Value);

        try
        {
            using var ms = new MemoryStream(imageBytes);
            ms.Position = 0;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = null;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        catch (NotSupportedException)
        {
            return null;
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    /// <summary>Converts the image to a Base64 data image string in the specified format.</summary>
    public static string ConvertToBase64DataImage(
        BitmapImage image,
        ImageFormatType imageFormatType)
        => image.ToBase64DataImage(imageFormatType);
}