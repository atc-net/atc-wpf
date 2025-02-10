namespace Atc.Wpf.Tests.ValueConverters;

public sealed class CollectionNullOrEmptyToVisibilityCollapsedValueConverterTests
{
    private readonly IValueConverter converter = new CollectionNullOrEmptyToVisibilityCollapsedValueConverter();

    [Theory]
    [InlineData(Visibility.Collapsed, null)]
    [InlineData(Visibility.Collapsed, new object[] { })]
    [InlineData(Visibility.Visible, new object[] { 1 })]
    public void Convert(Visibility expected, object? input)
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