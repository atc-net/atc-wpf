// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class StandardNumericFormatTypeToFormatStringValueConverterTests
{
    private readonly IValueConverter converter = new StandardNumericFormatTypeToFormatStringValueConverter();

    [Theory]
    [InlineData("G", StandardNumericFormatType.General)]
    [InlineData("C", StandardNumericFormatType.Currency)]
    [InlineData("D1", StandardNumericFormatType.Decimal1)]
    [InlineData("D2", StandardNumericFormatType.Decimal2)]
    [InlineData("F1", StandardNumericFormatType.FixedPoint1)]
    [InlineData("F2", StandardNumericFormatType.FixedPoint2)]
    [InlineData("N3", StandardNumericFormatType.Number3)]
    [InlineData("N4", StandardNumericFormatType.Number4)]
    [InlineData("P", StandardNumericFormatType.Percent)]
    public void Convert(
        string expected,
        StandardNumericFormatType input)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: null,
                culture: null));

    [Fact]
    public void Convert_Null_Throws_ArgumentNullException()
    {
        // Act
        var exception = Record.Exception(
            () => converter.Convert(
                value: null,
                targetType: null,
                parameter: null,
                culture: null));

        // Assert
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ConvertBack_Should_Throw_Exception()
    {
        // Act
        var exception = Record.Exception(
            () => converter.ConvertBack(
                value: null,
                targetType: null,
                parameter: null,
                culture: null));

        // Assert
        Assert.IsType<NotSupportedException>(exception);
    }
}