// ReSharper disable PossibleNullReferenceException
namespace Atc.Wpf.Tests.ValueConverters;

[Collection("Sequential")]
public sealed class ColorToBrushValueConverterTests
{
    private readonly IValueConverter converter = new ColorToBrushValueConverter();

    [Theory]
    [InlineData("#FFFF1493", null)]
    [InlineData("#FFFF1493", "#FFFF1493")]
    [InlineData("#FF000000", "#FF000000")]
    public void Convert(
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
        var actual = converter.Convert(
            input,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<SolidColorBrush>(actual);
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
        SolidColorBrush? input = null;
        if (inputHex is not null)
        {
            input = (SolidColorBrush)new BrushConverter().ConvertFrom(inputHex);
        }

        // Act
        var actual = converter.ConvertBack(
            input,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<Color>(actual);
        Assert.Equal(expectedHex, actual.ToString());
    }

    [Fact]
    public void Convert_NonColorValue_ReturnsBindingFallbacksBrush()
    {
        try
        {
            BindingFallbacks.Reset();

            var actual = converter.Convert(
                value: "not a color",
                targetType: null,
                parameter: null,
                culture: null);

            var brush = Assert.IsType<SolidColorBrush>(actual);
            Assert.Equal(BindingFallbacks.Color, brush.Color);
        }
        finally
        {
            BindingFallbacks.Reset();
        }
    }

    [Fact]
    public void ConvertBack_NonBrushValue_ReturnsBindingFallbacksColor()
    {
        try
        {
            BindingFallbacks.Reset();

            var actual = converter.ConvertBack(
                value: "not a brush",
                targetType: null,
                parameter: null,
                culture: null);

            var color = Assert.IsType<Color>(actual);
            Assert.Equal(BindingFallbacks.Color, color);
        }
        finally
        {
            BindingFallbacks.Reset();
        }
    }
}