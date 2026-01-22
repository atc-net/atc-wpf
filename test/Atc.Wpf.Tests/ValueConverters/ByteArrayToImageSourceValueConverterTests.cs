// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ByteArrayToImageSourceValueConverterTests
{
    private readonly IValueConverter converter = new ByteArrayToImageSourceValueConverter();

    [Fact]
    public void Convert_WithNullValue_ReturnsNull()
    {
        var result = converter.Convert(value: null, targetType: null, parameter: null, culture: null);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_WithEmptyByteArray_ReturnsNull()
    {
        var result = converter.Convert(value: Array.Empty<byte>(), targetType: null, parameter: null, culture: null);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_WithNonByteArrayValue_ReturnsNull()
    {
        var result = converter.Convert(value: "not a byte array", targetType: null, parameter: null, culture: null);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_WithInvalidImageData_ReturnsNull()
    {
        var invalidBytes = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        var result = converter.Convert(value: invalidBytes, targetType: null, parameter: null, culture: null);

        Assert.Null(result);
    }

    [StaFact]
    public void Convert_WithValidPngData_ReturnsBitmapImage()
    {
        // Create a minimal 1x1 white PNG
        var pngBytes = CreateMinimalPng();
        var result = converter.Convert(value: pngBytes, targetType: null, parameter: null, culture: null);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<BitmapImage>(result);
    }

    [Fact]
    public void ConvertBack_WithNullValue_ReturnsNull()
    {
        var result = converter.ConvertBack(value: null, targetType: null, parameter: null, culture: null);

        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_WithNonBitmapSourceValue_ReturnsNull()
    {
        var result = converter.ConvertBack(value: "not a bitmap", targetType: null, parameter: null, culture: null);

        Assert.Null(result);
    }

    private static byte[] CreateMinimalPng()
        =>
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
            0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53,
            0xDE, 0x00, 0x00, 0x00, 0x0C, 0x49, 0x44, 0x41,
            0x54, 0x08, 0xD7, 0x63, 0xF8, 0xFF, 0xFF, 0xFF,
            0x00, 0x05, 0xFE, 0x02, 0xFE, 0xDC, 0xCC, 0x59,
            0xE7, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E,
            0x44, 0xAE, 0x42, 0x60, 0x82,
        ];
}