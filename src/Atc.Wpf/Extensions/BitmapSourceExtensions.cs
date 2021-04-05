using System.ComponentModel;
using System.IO;
using System.Reflection;
using Atc;
using Atc.Wpf.Factories;

// ReSharper disable once CheckNamespace
namespace System.Windows.Media.Imaging
{
    public static class BitmapSourceExtensions
    {
        public static int GetBytesPerPixel(this BitmapSource bitmapSource)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            bitmapSource = bitmapSource.EnsureRelativeUriLocation();

            return (bitmapSource.Format.BitsPerPixel + 7) / 8;
        }

        public static int GetStride(this BitmapSource bitmapSource)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            bitmapSource = bitmapSource.EnsureRelativeUriLocation();

            var bytesForPixelWidth = bitmapSource.PixelWidth * bitmapSource.GetBytesPerPixel();
            return 4 * ((bytesForPixelWidth + 3) / 4);
        }

        public static byte[] GetBytes(this BitmapSource bitmapSource)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            bitmapSource = bitmapSource.EnsureRelativeUriLocation();

            var height = bitmapSource.PixelHeight;
            var stride = bitmapSource.GetStride();
            var pixels = new byte[height * stride];
            bitmapSource.CopyPixels(pixels, stride, 0);
            return pixels;
        }

        public static BitmapSource EnsureRelativeUriLocation(this BitmapSource bitmapSource)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            var path = bitmapSource.ToString(GlobalizationConstants.EnglishCultureInfo);
            if (!path.StartsWith("..", StringComparison.Ordinal))
            {
                return bitmapSource;
            }

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = path.Replace("..", location, StringComparison.Ordinal);
                return BitmapImageFactory.Create(path, UriKind.Relative);
            }

            path = path.Replace("..", ".", StringComparison.Ordinal);
            return BitmapImageFactory.Create(path, UriKind.Relative);
        }

        public static BitmapSource InvertColors(this BitmapSource bitmapSource)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            bitmapSource = bitmapSource.EnsureRelativeUriLocation();

            var stride = bitmapSource.GetStride();
            var length = stride * bitmapSource.PixelHeight;

            var data = new byte[length];
            bitmapSource.CopyPixels(data, stride, 0);

            for (var i = 0; i < length; i += 4)
            {
                data[i] = (byte)(255 - data[i]);            // R
                data[i + 1] = (byte)(255 - data[i + 1]);    // G
                data[i + 2] = (byte)(255 - data[i + 2]);    // B
                data[i + 3] = (byte)(255 - data[i + 3]);    // A
            }

            return BitmapSource.Create(
                bitmapSource.PixelWidth,
                bitmapSource.PixelHeight,
                bitmapSource.DpiX,
                bitmapSource.DpiY,
                bitmapSource.Format,
                palette: null,
                data,
                stride);
        }

        /// <summary>
        /// Save the bitmap into a file.
        /// </summary>
        /// <param name="bitmapSource">The bitmap.</param>
        /// <param name="fileName">The file.</param>
        public static void Save(this BitmapSource bitmapSource, string fileName)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            bitmapSource.Save(new FileInfo(fileName));
        }

        /// <summary>
        /// Save the bitmap into a file.
        /// </summary>
        /// <param name="bitmapSource">The bitmap.</param>
        /// <param name="fileInfo">The file information.</param>
        public static void Save(this BitmapSource bitmapSource, FileInfo fileInfo)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            if (fileInfo is null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            switch (fileInfo.Extension.ToLower(GlobalizationConstants.EnglishCultureInfo))
            {
                case ".bmp":
                    bitmapSource.Save(fileInfo, ImageFormatType.Bmp);
                    break;
                case ".gif":
                    bitmapSource.Save(fileInfo, ImageFormatType.Gif);
                    break;
                case ".jpg":
                case ".jpeg":
                    bitmapSource.Save(fileInfo, ImageFormatType.Jpeg);
                    break;
                case ".png":
                    bitmapSource.Save(fileInfo, ImageFormatType.Png);
                    break;
                case ".tif":
                case ".tiff":
                    bitmapSource.Save(fileInfo, ImageFormatType.Tiff);
                    break;
                case ".wmp":
                    bitmapSource.Save(fileInfo, ImageFormatType.Wmp);
                    break;
            }
        }

        /// <summary>
        /// Save the bitmap into a file.
        /// </summary>
        /// <param name="bitmapSource">The bitmap.</param>
        /// <param name="fileInfo">The file information.</param>
        /// <param name="imageFormatType">Type of the image format.</param>
        public static void Save(this BitmapSource bitmapSource, FileInfo fileInfo, ImageFormatType imageFormatType)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            if (fileInfo is null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            using FileStream stream = new FileStream(fileInfo.FullName, FileMode.Create);
            var encoder = BitmapEncoderFactory.Create(imageFormatType);
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(stream);
        }

        public static BitmapImage ToBitmapImage(this BitmapSource bitmapSource, ImageFormatType imageFormatType = ImageFormatType.Png, int? newWidth = null)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            bitmapSource = bitmapSource.EnsureRelativeUriLocation();

            var bitmapImage = new BitmapImage();
            var bitmapEncoder = BitmapEncoderFactory.Create(imageFormatType);
            var memoryStream = new MemoryStream();

            var bitmapFrame = BitmapFrame.Create(bitmapSource);
            bitmapEncoder.Frames.Add(bitmapFrame);
            bitmapEncoder.Save(memoryStream);

            memoryStream.Seek(0, SeekOrigin.Begin);
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            if (newWidth > 0)
            {
                bitmapImage.DecodePixelWidth = (int)newWidth;
            }

            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            memoryStream.Close();

            return bitmapImage;
        }

        public static BitmapImage ToResizedBitmapImage(this BitmapSource bitmapSource, int newWidth)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            bitmapSource = bitmapSource.EnsureRelativeUriLocation();

            return bitmapSource.ToBitmapImage(ImageFormatType.Png, newWidth);
        }

        public static BitmapSource ToBitmapSourceAsGray32(this BitmapSource bitmapSource)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            bitmapSource = bitmapSource.EnsureRelativeUriLocation();

            return bitmapSource.ToFormatConvertedBitmapAsGray32();
        }

        public static FormatConvertedBitmap ToFormatConvertedBitmapAsGray32(this BitmapSource bitmapSource)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            bitmapSource = bitmapSource.EnsureRelativeUriLocation();

            return new FormatConvertedBitmap(
                bitmapSource,
                PixelFormats.Gray32Float,
                destinationPalette: null,
                alphaThreshold: 0);
        }

        public static WriteableBitmap ToWriteableBitmap(this BitmapSource bitmapSource)
        {
            if (bitmapSource is null)
            {
                throw new ArgumentNullException(nameof(bitmapSource));
            }

            bitmapSource = bitmapSource.EnsureRelativeUriLocation();

            if (bitmapSource.Format == PixelFormats.Pbgra32)
            {
                return new WriteableBitmap(bitmapSource);
            }

            var formattedBitmapSource = new FormatConvertedBitmap();
            formattedBitmapSource.BeginInit();
            formattedBitmapSource.Source = bitmapSource;
            formattedBitmapSource.DestinationFormat = PixelFormats.Pbgra32;
            formattedBitmapSource.EndInit();
            return new WriteableBitmap(formattedBitmapSource);
        }
    }
}