using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Atc.Wpf.Factories;

namespace Atc.Wpf.Extensions
{
    public static class ImageExtensions
    {
        public static BitmapImage ToBitmapImage(this Image image)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (image.Source is RenderTargetBitmap renderTargetBitmap)
            {
                // Get the source bitmap from RenderTargetBitmap
                return renderTargetBitmap.ToBitmapImage();
            }

            if (image.Source.ToString(GlobalizationConstants.EnglishCultureInfo).Contains("/", StringComparison.Ordinal) ||
                image.Source.ToString(GlobalizationConstants.EnglishCultureInfo).Contains("\\", StringComparison.Ordinal))
            {
                // Get the source bitmap from Uri
                return BitmapImageFactory.Create(image.Source.ToString(GlobalizationConstants.EnglishCultureInfo));
            }

            if (image.Source is BitmapImage bitmapImage)
            {
                return bitmapImage;
            }

            throw new FormatException("image.Source");
        }

        public static Image AutoGreyImage(this Image image, bool isEnabled)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (isEnabled)
            {
                var bitmapImage = image.Source is FormatConvertedBitmap
                    ? BitmapImageFactory.Create(image.Source.ToString(GlobalizationConstants.EnglishCultureInfo))
                    : ((FormatConvertedBitmap)image.Source).Source;

                // Set the Source property to the original value.
                image.SetCurrentValue(
                    Image.SourceProperty,
                    bitmapImage);

                // Reset the Opacity Mask
                image.SetCurrentValue(
                    UIElement.OpacityMaskProperty,
                    value: null);
            }
            else
            {
                // Get the source bitmap
                var bitmapImage = image.Source is FormatConvertedBitmap
                    ? BitmapImageFactory.Create(image.Source.ToString(GlobalizationConstants.EnglishCultureInfo))
                    : BitmapImageFactory.Create(((FormatConvertedBitmap)image.Source).Source.ToString(GlobalizationConstants.EnglishCultureInfo));

                // Convert it to Gray
                image.SetCurrentValue(
                    Image.SourceProperty,
                    bitmapImage.ToFormatConvertedBitmapAsGray32());

                // Create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
                image.SetCurrentValue(
                    UIElement.OpacityMaskProperty,
                    new ImageBrush(bitmapImage));
            }

            return image;
        }
    }
}