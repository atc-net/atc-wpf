// ReSharper disable once CheckNamespace
namespace System.Windows.Controls;

/// <summary>
/// Extension methods for <see cref="Image"/>.
/// </summary>
public static class ImageExtensions
{
    private static readonly ConditionalWeakTable<FormatConvertedBitmap, BitmapSource> OriginalSources = new();

    /// <summary>
    /// Gets a <see cref="BitmapImage"/> for the image's source.
    /// </summary>
    public static BitmapImage ToBitmapImage(this Image image)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (image.Source is RenderTargetBitmap renderTargetBitmap)
        {
            // Get the source bitmap from RenderTargetBitmap
            return renderTargetBitmap.ToBitmapImage();
        }

        if (image.Source
                .ToString(GlobalizationConstants.EnglishCultureInfo)
                .Contains('/', StringComparison.Ordinal) ||
            image.Source
                .ToString(GlobalizationConstants.EnglishCultureInfo)
                .Contains('\\', StringComparison.Ordinal))
        {
            // Get the source bitmap from Uri
            return BitmapImageFactory.Create(
                image.Source.ToString(GlobalizationConstants.EnglishCultureInfo));
        }

        if (image.Source is BitmapImage bitmapImage)
        {
            return bitmapImage;
        }

        throw new FormatException("image.Source");
    }

    /// <summary>
    /// Restores the original image when enabled, or shows a grayscale version with an opacity mask when disabled.
    /// </summary>
    /// <remarks>
    /// Only bitmap sources can be shown in grayscale; any other source is left unchanged.
    /// </remarks>
    public static Image AutoGrey(
        this Image image,
        bool isEnabled)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (isEnabled)
        {
            if (image.Source is not FormatConvertedBitmap greyBitmap)
            {
                return image;
            }

            var originalSource = OriginalSources.TryGetValue(greyBitmap, out var source)
                ? source
                : greyBitmap.Source;

            // Set the Source property to the original value.
            image.SetCurrentValue(
                Image.SourceProperty,
                originalSource);

            // Reset the Opacity Mask
            image.SetCurrentValue(
                UIElement.OpacityMaskProperty,
                value: null);
        }
        else
        {
            if (image.Source is FormatConvertedBitmap or not BitmapSource)
            {
                return image;
            }

            var bitmapSource = (BitmapSource)image.Source;
            var grey = bitmapSource.ToFormatConvertedBitmapAsGray32();
            OriginalSources.AddOrUpdate(grey, bitmapSource);

            // Convert it to Gray
            image.SetCurrentValue(
                Image.SourceProperty,
                grey);

            // Create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
            image.SetCurrentValue(
                UIElement.OpacityMaskProperty,
                new ImageBrush(bitmapSource));
        }

        return image;
    }
}