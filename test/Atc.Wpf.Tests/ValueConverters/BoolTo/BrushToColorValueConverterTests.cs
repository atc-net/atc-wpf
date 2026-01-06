// ReSharper disable PossibleNullReferenceException
// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class BrushToColorValueConverterTests
{
    private readonly IValueConverter converter = new BrushToColorValueConverter();

    [Theory]
    [InlineData("#FFFF1493", null)]
    [InlineData("#FFFF1493", "#FFFF1493")]
    [InlineData("#FF000000", "#FF000000")]
    public void Convert(
        string expectedHex,
        string? inputHex)
    {
        // Arrange
        SolidColorBrush? input = null;
        if (inputHex is not null)
        {
            input = (SolidColorBrush)new BrushConverter().ConvertFrom(inputHex);
        }

        // Act
        var actual = converter.Convert(
            input,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<Color>(actual);
        Assert.Equal(expectedHex, actual.ToString());
    }

    [Theory]
    [InlineData("#FFFF1493", null)]
    [InlineData("#FFFF1493", "#FFFF1493")]
    [InlineData("#FF000000", "#FF000000")]
    public void ConvertBack(
        string expectedHex,
        string? inputHex)
    {
        // Arrange
        Color? input = null;
        if (inputHex is not null)
        {
            input = (Color)ColorConverter.ConvertFromString(inputHex);
        }

        // Act
        var actual = converter.ConvertBack(
            input,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<SolidColorBrush>(actual);
        Assert.Equal(expectedHex, actual.ToString());
    }
}