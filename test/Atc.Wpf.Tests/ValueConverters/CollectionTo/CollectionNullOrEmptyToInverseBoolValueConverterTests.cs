// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class CollectionNullOrEmptyToInverseBoolValueConverterTests
{
    private readonly IValueConverter converter = new CollectionNullOrEmptyToInverseBoolValueConverter();

    [Theory]
    [InlineData(false, null)]
    [InlineData(false, new object[] { })]
    [InlineData(true, new object[] { 1 })]
    public void Convert(bool expected, object? input)
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