// ReSharper disable StringLiteralTypo
// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class BrushToColorNameValueConverterTests
{
    private readonly IValueConverter converter = new BrushToColorNameValueConverter();

    [Theory]
    [InlineData("Red", "#FFFF0000")]
    [InlineData("Black", "#FF000000")]
    [InlineData("White", "#FFFFFFFF")]
    [InlineData("Gainsboro", "#FFDCDCDC")]
    [InlineData("IndianRed", "#FFCD5C5C")]
    public void Convert_KnownBrush_ReturnsKey(
        string expectedKey,
        string inputHex)
    {
        // Arrange
        var brush = (SolidColorBrush)new BrushConverter().ConvertFrom(inputHex)!;

        // Act
        var actual = converter.Convert(
            brush,
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(expectedKey, actual);
    }

    [Fact]
    public void Convert_UnknownBrush_ReturnsEmptyString()
    {
        // Arrange — a color that does not match any well-known palette entry.
        var brush = new SolidColorBrush(Color.FromArgb(0xFF, 0x12, 0x34, 0x56));

        // Act
        var actual = converter.Convert(
            brush,
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void Convert_NullValue_ReturnsEmptyString()
    {
        // Act
        var actual = converter.Convert(
            value: null,
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void Convert_NonBrushValue_ReturnsEmptyString()
    {
        // Act
        var actual = converter.Convert(
            value: 42,
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(string.Empty, actual);
    }

    [Theory]
    [InlineData("Red", "#FFFF0000")]
    [InlineData("Black", "#FF000000")]
    [InlineData("White", "#FFFFFFFF")]
    public void ConvertBack_KnownKey_ReturnsBrush(
        string inputKey,
        string expectedHex)
    {
        // Act
        var actual = converter.ConvertBack(
            inputKey,
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        var brush = Assert.IsType<SolidColorBrush>(actual);
        Assert.Equal(expectedHex, brush.ToString(GlobalizationConstants.EnglishCultureInfo));
    }

    [Fact]
    public void ConvertBack_EmptyString_ReturnsBindingDoNothing()
    {
        // Act
        var actual = converter.ConvertBack(
            string.Empty,
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Same(Binding.DoNothing, actual);
    }

    [Fact]
    public void ConvertBack_NullValue_ReturnsBindingDoNothing()
    {
        // Act
        var actual = converter.ConvertBack(
            value: null,
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Same(Binding.DoNothing, actual);
    }

    [Fact]
    public void ConvertBack_UnknownKey_ReturnsBindingDoNothing()
    {
        // Act — a sentinel that does not match any well-known color.
        var actual = converter.ConvertBack(
            "NotAColorName",
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Same(Binding.DoNothing, actual);
    }

    [Fact]
    public void ConvertBack_NonStringValue_ReturnsBindingDoNothing()
    {
        // Act
        var actual = converter.ConvertBack(
            value: 42,
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Same(Binding.DoNothing, actual);
    }
}