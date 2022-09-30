namespace Atc.Wpf.Tests.ValueConverters;

public class StringNullOrEmptyToVisibilityCollapsedValueConverterTests
{
    private readonly IValueConverter converter = new StringNullOrEmptyToVisibilityCollapsedValueConverter();

    [Theory]
    [InlineData(Visibility.Collapsed, null)]
    [InlineData(Visibility.Collapsed, "")]
    [InlineData(Visibility.Visible, "Hello")]
    public void Convert(Visibility expected, string? input)
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