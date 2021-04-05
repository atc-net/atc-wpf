using System;
using System.Windows.Media.Imaging;

// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
namespace Atc.Wpf.Factories
{
    public static class BitmapEncoderFactory
    {
        public static BitmapEncoder Create(ImageFormatType imageFormatType)
        {
            return imageFormatType switch
            {
                ImageFormatType.Bmp => new BmpBitmapEncoder(),
                ImageFormatType.Gif => new GifBitmapEncoder(),
                ImageFormatType.Jpeg => new JpegBitmapEncoder(),
                ImageFormatType.Png => new PngBitmapEncoder(),
                ImageFormatType.Tiff => new TiffBitmapEncoder(),
                ImageFormatType.Wmp => new WmpBitmapEncoder(),
                _ => throw new SwitchCaseDefaultException(imageFormatType)
            };
        }
    }
}