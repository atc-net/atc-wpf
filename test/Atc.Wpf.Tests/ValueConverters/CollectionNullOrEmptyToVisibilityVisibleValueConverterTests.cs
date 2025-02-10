namespace Atc.Wpf.Tests.ValueConverters;

public sealed class CollectionNullOrEmptyToVisibilityVisibleValueConverterTests
{
    private readonly IValueConverter converter = new CollectionNullOrEmptyToVisibilityVisibleValueConverter();

    [Theory]
    [InlineData(Visibility.Visible, null)]
    [InlineData(Visibility.Visible, new object[] { })]
    [InlineData(Visibility.Collapsed, new object[] { 1 })]
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