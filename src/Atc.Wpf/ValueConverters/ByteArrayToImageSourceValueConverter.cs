namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Byte Array To ImageSource.
/// </summary>
/// <remarks>
/// Converts a byte array containing image data to a <see cref="BitmapImage"/> that can be used as an ImageSource.
/// Supports common image formats (PNG, JPEG, GIF, BMP, etc.).
/// Returns null if the byte array is null or empty.
/// </remarks>
/// <example>
/// <code>
/// &lt;Image Source="{Binding ImageData, Converter={x:Static converters:ByteArrayToImageSourceValueConverter.Instance}}" /&gt;
/// </code>
/// </example>
[ValueConversion(typeof(byte[]), typeof(ImageSource))]
public sealed class ByteArrayToImageSourceValueConverter : IValueConverter
{
    public static readonly ByteArrayToImageSourceValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not byte[] bytes || bytes.Length == 0)
        {
            return null;
        }

        try
        {
            using var memoryStream = new MemoryStream(bytes);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch
#pragma warning restore CA1031 // Do not catch general exception types
        {
            return null;
        }
    }

    /// <inheritdoc />
    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not BitmapSource bitmapSource)
        {
            return null;
        }

        try
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using var memoryStream = new MemoryStream();
            encoder.Save(memoryStream);
            return memoryStream.ToArray();
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch
#pragma warning restore CA1031 // Do not catch general exception types
        {
            return null;
        }
    }
}