// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ObjectNullToVisibilityVisibleValueConverterTests
{
    private readonly IValueConverter converter = new ObjectNullToVisibilityVisibleValueConverter();

    [Theory]
    [InlineData(Visibility.Visible, null)]
    [InlineData(Visibility.Collapsed, true)]
    [InlineData(Visibility.Collapsed, "Hello")]
    public void Convert(
        Visibility expected,
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
        var exception = Record.Exception(() => converter.ConvertBack(
            value: null,
            targetType: null,
            parameter: null,
            culture: null));

        // Assert
        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}