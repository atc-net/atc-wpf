namespace Atc.Wpf.Factories;

/// <summary>
/// Factory for creating <see cref="BitmapImage"/> instances.
/// </summary>
public static class BitmapImageFactory
{
    /// <summary>
    /// Creates a frozen <see cref="BitmapImage"/> from the specified URI location.
    /// </summary>
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "OK.")]
    public static BitmapImage Create(
        string uriLocation,
        UriKind uriKind = UriKind.RelativeOrAbsolute)
    {
        ArgumentNullException.ThrowIfNull(uriLocation);

        var uri = new Uri(uriLocation, uriKind);

        if (uriKind is UriKind.Absolute)
        {
            // Load synchronously and freeze so the bitmap is thread-safe
            // and avoids per-access copy-on-write costs.
            var absoluteImage = new BitmapImage();
            absoluteImage.BeginInit();
            absoluteImage.UriSource = uri;
            absoluteImage.CacheOption = BitmapCacheOption.OnLoad;
            absoluteImage.EndInit();
            absoluteImage.Freeze();
            return absoluteImage;
        }

        // Relative path: ToBitmapImage() re-encodes through a MemoryStream
        // and freezes the result, so no extra Freeze call is needed here.
        return new BitmapImage(uri)
            .EnsureRelativeUriLocation()
            .ToBitmapImage();
    }
}