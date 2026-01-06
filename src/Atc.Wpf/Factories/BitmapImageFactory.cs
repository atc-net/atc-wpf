namespace Atc.Wpf.Factories;

public static class BitmapImageFactory
{
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "OK.")]
    public static BitmapImage Create(
        string uriLocation,
        UriKind uriKind = UriKind.RelativeOrAbsolute)
    {
        ArgumentNullException.ThrowIfNull(uriLocation);

        var bitmapImage = new BitmapImage(
            new Uri(
                uriLocation,
                uriKind));

        return uriKind is UriKind.Absolute
            ? bitmapImage
            : bitmapImage
                .EnsureRelativeUriLocation()
                .ToBitmapImage();
    }
}