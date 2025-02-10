namespace Atc.Wpf.Controls.Media;

/// <summary>
/// Auto Greyable Image.
/// </summary>
public sealed class AutoGreyableImage : Image
{
    private static readonly ConcurrentDictionary<int, FormatConvertedBitmap> CacheFormatConvertedBitmap = new();
    private static readonly ConcurrentDictionary<int, ImageBrush> CacheImageBrush = new();

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
        var autoGreyableImage = d as AutoGreyableImage;
        if (autoGreyableImage?.Source is null ||
            autoGreyableImage.Source is FormatConvertedBitmap)
        {
            return;
        }

        HandleAutoGreyableImage(autoGreyableImage, autoGreyableImage.IsEnabled);
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
        if (isEnable)
        {
            // Set the Source property to the original value.
            if (autoGreyableImage is not { Source: FormatConvertedBitmap formatConvertedBitmap })
            {
                return;
            }

            autoGreyableImage.Source = formatConvertedBitmap.Source;

            // Reset the Opacity Mask
            autoGreyableImage.OpacityMask = null;
        }
        else
        {
            var bitmapImage = autoGreyableImage.ToBitmapImage();
            var bitmapImageHashCode = bitmapImage.GetHashCode();

            // Convert it to Gray
            autoGreyableImage.Source = CacheFormatConvertedBitmap.GetOrAdd(
                bitmapImageHashCode,
                bitmapImage.ToFormatConvertedBitmapAsGray32());

            // Create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
            autoGreyableImage.OpacityMask = CacheImageBrush.GetOrAdd(bitmapImageHashCode, new ImageBrush(bitmapImage));
        }
    }
}