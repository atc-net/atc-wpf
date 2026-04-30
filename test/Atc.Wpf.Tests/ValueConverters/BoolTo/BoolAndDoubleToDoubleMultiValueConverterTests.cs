// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class BoolAndDoubleToDoubleMultiValueConverterTests
{
    private readonly IMultiValueConverter converter = new BoolAndDoubleToDoubleMultiValueConverter();

    [Theory]
    [InlineData(0d, false, 150d)]
    [InlineData(150d, true, 150d)]
    [InlineData(0d, true, null)]
    [InlineData(0d, null, 150d)]
    public void Convert(
        double expected,
        bool? boolInput,
        object? doubleInput)
    {
        // Arrange
        var values = new object?[] { boolInput, doubleInput };

        // Act
        var actual = converter.Convert(
            values,
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Convert_FloatAndIntInputs_AreCoercedToDouble()
    {
        // Act
        var fromFloat = converter.Convert(
            new object?[] { true, 12.5f },
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        var fromInt = converter.Convert(
            new object?[] { true, 200 },
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(12.5d, fromFloat);
        Assert.Equal(200d, fromInt);
    }

    [Fact]
    public void Convert_TooFewValues_ReturnsZero()
    {
        // Act
        var actual = converter.Convert(
            new object?[] { true },
            targetType: null!,
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(0d, actual);
    }

    [Fact]
    public void ConvertBack_Should_Throw_Exception()
    {
        // Act
        var exception = Record.Exception(
            () => converter.ConvertBack(
                value: null!,
                targetTypes: null!,
                parameter: null,
                culture: null!));

        // Assert
        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}