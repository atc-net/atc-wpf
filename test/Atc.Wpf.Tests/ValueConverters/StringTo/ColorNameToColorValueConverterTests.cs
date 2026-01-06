namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ColorNameToColorValueConverterTests
{
    private readonly IValueConverter converter = new ColorNameToColorValueConverter();

    [Theory]
    [InlineData("#FFFF1493", null)]
    [InlineData("#FFFF0000", "Red")]
    [InlineData("#FF008000", "Green")]
    [InlineData("#FF0000FF", "Blue")]
    public void Convert(
        string expectedHex,
        string? input)
    {
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
    [InlineData("DeepPink", null)]
    [InlineData("Red", "#FFFF0000")]
    [InlineData("Green", "#FF008000")]
    [InlineData("Blue", "#FF0000FF")]
    [InlineData("#FF0000F0", "#FF0000F0")]
    public void ConvertBack(
        string expectedHex,
        string? inputName)
    {
        // Arrange
        Color? input = null;
        if (inputName is not null)
        {
            input = (Color)ColorConverter.ConvertFromString(inputName)!;
        }

        // Act
        var actual = converter.ConvertBack(
            input,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<string>(actual);
        Assert.Equal(expectedHex, actual.ToString());
    }
}