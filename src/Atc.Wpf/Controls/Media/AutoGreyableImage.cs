// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
namespace Atc.Wpf.Controls.Media;

/// <summary>
/// Auto Greyable Image.
/// </summary>
/// <remarks>
/// When disabled, the image shows a greyscale version of its source. Greyscale versions are cached per source
/// instance (shared by every image using that source, reused on every toggle) and are released together with
/// the source.
/// </remarks>
public sealed class AutoGreyableImage : Image
{
    private static readonly ConditionalWeakTable<ImageSource, GreyVersion> GreyVersions = new();

    private ImageSource? originalSource;
    private bool isSwappingSource;

    /// <summary>
    /// Initializes static members of the <see cref="AutoGreyableImage"/> class.
    /// </summary>
    static AutoGreyableImage()
    {
        IsEnabledProperty.OverrideMetadata(
            typeof(AutoGreyableImage),
            new FrameworkPropertyMetadata(
                defaultValue: true,
                OnIsEnabledChanged));

        SourceProperty.OverrideMetadata(
            typeof(AutoGreyableImage),
            new FrameworkPropertyMetadata(
                defaultValue: null,
                OnSourceChanged));
    }

    /// <summary>
    /// Called when [is source changed].
    /// </summary>
    /// <param name="d">The dependency object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void OnSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not AutoGreyableImage autoGreyableImage ||
            autoGreyableImage.isSwappingSource)
        {
            return;
        }

        // A source set by the consumer becomes the original to restore when the image is enabled again.
        autoGreyableImage.originalSource = autoGreyableImage.Source;
        if (autoGreyableImage.Source is not null)
        {
            HandleAutoGreyableImage(autoGreyableImage, autoGreyableImage.IsEnabled);
        }
    }

    /// <summary>
    /// Called when [is enabled changed].
    /// </summary>
    /// <param name="d">The dependency object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void OnIsEnabledChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var autoGreyableImage = d as AutoGreyableImage;
        if (autoGreyableImage?.Source is null)
        {
            return;
        }

        var isEnable = e.NewValue is not null &&
                       Convert.ToBoolean(e.NewValue, GlobalizationConstants.EnglishCultureInfo);

        HandleAutoGreyableImage(autoGreyableImage, isEnable);
    }

    private static void HandleAutoGreyableImage(
        AutoGreyableImage autoGreyableImage,
        bool isEnable)
    {
        var original = autoGreyableImage.originalSource;
        if (original is null)
        {
            return;
        }

        if (isEnable)
        {
            autoGreyableImage.SwapSource(original);

            // Reset the Opacity Mask
            autoGreyableImage.OpacityMask = null;
        }
        else
        {
            var grey = GreyVersions.GetValue(original, CreateGreyVersion);

            autoGreyableImage.SwapSource(grey.Bitmap);

            // Opacity mask for the greyscale image, as FormatConvertedBitmap does not keep transparency info.
            autoGreyableImage.OpacityMask = grey.OpacityMask;
        }
    }

    private static GreyVersion CreateGreyVersion(ImageSource source)
    {
        var bitmapImage = new Image { Source = source }.ToBitmapImage();

        var bitmap = bitmapImage.ToFormatConvertedBitmapAsGray32();
        if (bitmap.CanFreeze)
        {
            bitmap.Freeze();
        }

        var opacityMask = new ImageBrush(bitmapImage);
        if (opacityMask.CanFreeze)
        {
            opacityMask.Freeze();
        }

        return new GreyVersion(bitmap, opacityMask);
    }

    private void SwapSource(ImageSource source)
    {
        if (ReferenceEquals(Source, source))
        {
            return;
        }

        isSwappingSource = true;
        try
        {
            Source = source;
        }
        finally
        {
            isSwappingSource = false;
        }
    }

    private sealed record GreyVersion(
        FormatConvertedBitmap Bitmap,
        ImageBrush OpacityMask);
}