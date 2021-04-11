using System;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Atc.Wpf.Extensions;

namespace Atc.Wpf.Controls
{
    /// <summary>
    /// Auto Greyable Image.
    /// </summary>
    public class AutoGreyableImage : Image
    {
        private static readonly ConcurrentDictionary<int, FormatConvertedBitmap> CacheFormatConvertedBitmap = new ConcurrentDictionary<int, FormatConvertedBitmap>();
        private static readonly ConcurrentDictionary<int, ImageBrush> CacheImageBrush = new ConcurrentDictionary<int, ImageBrush>();

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
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var autoGreyScaleImage = obj as AutoGreyableImage;
            if (autoGreyScaleImage?.Source is null)
            {
                return;
            }

            bool isEnable = args.NewValue != null &&
                            Convert.ToBoolean(args.NewValue, GlobalizationConstants.EnglishCultureInfo);
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
}