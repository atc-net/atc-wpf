using System;
using System.Collections.Concurrent;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            IsEnabledProperty.OverrideMetadata(typeof(AutoGreyableImage), new FrameworkPropertyMetadata(true, OnIsEnabledChanged));
        }

        /// <summary>
        /// Called when [auto grey scale image is enabled property changed].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnIsEnabledChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var autoGreyScaleImg = source as AutoGreyableImage;
            if (autoGreyScaleImg?.Source is null)
            {
                return;
            }

            bool isEnable = args.NewValue != null &&
                            Convert.ToBoolean(args.NewValue, GlobalizationConstants.EnglishCultureInfo);
            if (isEnable)
            {
                // Set the Source property to the original value.
                if (!(autoGreyScaleImg.Source is FormatConvertedBitmap formatConvertedBitmap))
                {
                    return;
                }

                autoGreyScaleImg.Source = formatConvertedBitmap.Source;

                // Reset the Opacity Mask
                autoGreyScaleImg.OpacityMask = null;
            }
            else
            {
                BitmapImage bitmapImage;
                if (autoGreyScaleImg.Source is RenderTargetBitmap renderTargetBitmap)
                {
                    // Get the source bitmap from RenderTargetBitmap
                    bitmapImage = GetBitmapImageFromRenderTargetBitmap(renderTargetBitmap);
                }
                else if (autoGreyScaleImg.Source.ToString(GlobalizationConstants.EnglishCultureInfo).Contains("/", StringComparison.Ordinal) ||
                         autoGreyScaleImg.Source.ToString(GlobalizationConstants.EnglishCultureInfo).Contains("\\", StringComparison.Ordinal))
                {
                    // Get the source bitmap from Uri
                    bitmapImage = new BitmapImage(new Uri(autoGreyScaleImg.Source.ToString(GlobalizationConstants.EnglishCultureInfo)));
                }
                else
                {
                    if (autoGreyScaleImg.Source is BitmapImage tmpBitmapImage)
                    {
                        bitmapImage = tmpBitmapImage;
                    }
                    else
                    {
                        throw new FormatException("autoGreyScaleImg.Source");
                    }
                }

                var bitmapImageHashCode = bitmapImage.GetHashCode();

                // Convert it to Gray
                autoGreyScaleImg.Source = CacheFormatConvertedBitmap.GetOrAdd(bitmapImageHashCode, new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0));

                // Create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
                autoGreyScaleImg.OpacityMask = CacheImageBrush.GetOrAdd(bitmapImageHashCode, new ImageBrush(bitmapImage));
            }
        }

        private static BitmapImage GetBitmapImageFromRenderTargetBitmap(RenderTargetBitmap renderTargetBitmap)
        {
            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using MemoryStream stream = new MemoryStream();
            bitmapEncoder.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}