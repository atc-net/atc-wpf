namespace Atc.Wpf.Controls;

/// <summary>
/// Auto Greyable Image.
/// </summary>
public class AutoGreyableImage : Image
{
    private static readonly ConcurrentDictionary<int, FormatConvertedBitmap> CacheFormatConvertedBitmap = new();
    private static readonly ConcurrentDictionary<int, ImageBrush> CacheImageBrush = new();

    /// <summary>
    /// Initializes static members of the <see cref="AutoGreyableImage"/> class.
    /// </summary>
    static AutoGreyableImage()
    {
        // Override the metadata of the IsEnabled property.
        IsEnabledProperty.OverrideMetadata(
            typeof(AutoGreyableImage),
            new FrameworkPropertyMetadata(
                defaultValue: true,
                OnIsEnabledChanged));
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
        var autoGreyScaleImage = d as AutoGreyableImage;
        if (autoGreyScaleImage?.Source is null)
        {
            return;
        }

        var isEnable = e.NewValue is not null &&
                       Convert.ToBoolean(e.NewValue, GlobalizationConstants.EnglishCultureInfo);
        if (isEnable)
        {
            // Set the Source property to the original value.
            if (!(autoGreyScaleImage is { Source: FormatConvertedBitmap formatConvertedBitmap }))
            {
                return;
            }

            autoGreyScaleImage.Source = formatConvertedBitmap.Source;

            // Reset the Opacity Mask
            autoGreyScaleImage.OpacityMask = null;
        }
        else
        {
            var bitmapImage = autoGreyScaleImage.ToBitmapImage();
            var bitmapImageHashCode = bitmapImage.GetHashCode();

            // Convert it to Gray
            autoGreyScaleImage.Source = CacheFormatConvertedBitmap.GetOrAdd(
                bitmapImageHashCode,
                bitmapImage.ToFormatConvertedBitmapAsGray32());

            // Create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
            autoGreyScaleImage.OpacityMask = CacheImageBrush.GetOrAdd(bitmapImageHashCode, new ImageBrush(bitmapImage));
        }
    }
}