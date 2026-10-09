// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
namespace Atc.Wpf.Factories;

/// <summary>
/// Factory for creating <see cref="BitmapEncoder"/> instances.
/// </summary>
public static class BitmapEncoderFactory
{
    /// <summary>
    /// Creates a <see cref="BitmapEncoder"/> for the specified image format.
    /// </summary>
    public static BitmapEncoder Create(ImageFormatType imageFormatType)
        => imageFormatType switch
        {
            ImageFormatType.Bmp => new BmpBitmapEncoder(),
            ImageFormatType.Gif => new GifBitmapEncoder(),
            ImageFormatType.Jpeg => new JpegBitmapEncoder(),
            ImageFormatType.Png => new PngBitmapEncoder(),
            ImageFormatType.Tiff => new TiffBitmapEncoder(),
            ImageFormatType.Wmp => new WmpBitmapEncoder(),
            _ => throw new SwitchCaseDefaultException(imageFormatType),
        };
}