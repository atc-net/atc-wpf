// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class CollectionNullOrEmptyToBoolValueConverterTests
{
    private readonly IValueConverter converter = new CollectionNullOrEmptyToBoolValueConverter();

    [Theory]
    [InlineData(true, null)]
    [InlineData(true, new object[] { })]
    [InlineData(false, new object[] { 1 })]
    public void Convert(
        bool expected,
        object? input)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: null,
                culture: null));

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
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}