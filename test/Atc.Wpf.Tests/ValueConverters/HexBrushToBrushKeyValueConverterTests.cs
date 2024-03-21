// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Tests.ValueConverters;

public class HexBrushToBrushKeyValueConverterTests
{
    private readonly IValueConverter converter = new HexBrushToBrushKeyValueConverter();

    [Theory]
    [InlineData("Gainsboro", "#FFDCDCDC")]
    [InlineData("IndianRed", "#FFCD5C5C")]
    public void Convert(string expected, string? input)
    {
        // Act
        var actual = converter.Convert(input, targetType: null, parameter: null, culture: null);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<string>(actual);
        Assert.Equal(expected, actual.ToString());
    }

    [Theory]
    [InlineData("#FFDCDCDC", "Gainsboro")]
    [InlineData("#FFCD5C5C", "Indian Red")]
    public void ConvertBack(string expected, string? input)
    {
        // Act
        var actual = converter.ConvertBack(input, targetType: null, parameter: null, culture: null);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<SolidColorBrush>(actual);
        Assert.Equal(expected, actual.ToString());
    }
}