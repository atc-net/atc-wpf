namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ObjectNotNullToBoolValueConverterTests
{
    private readonly IValueConverter converter = new ObjectNotNullToBoolValueConverter();

    [Theory]
    [InlineData(false, null)]
    [InlineData(true, "")]
    [InlineData(true, "Hello")]
    public void Convert(bool expected, string? input)
        => Assert.Equal(
            expected,
            converter.Convert(input, targetType: null, parameter: null, culture: null));

    [Fact]
    public void ConvertBack_Should_Throw_Exception()
    {
        // Act
        var exception = Record.Exception(() => converter.ConvertBack(value: null, targetType: null, parameter: null, culture: null));

        // Assert
        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}