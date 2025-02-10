namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ObjectNotNullToVisibilityCollapsedValueConverterTests
{
    private readonly IValueConverter converter = new ObjectNotNullToVisibilityCollapsedValueConverter();

    [Theory]
    [InlineData(Visibility.Visible, null)]
    [InlineData(Visibility.Collapsed, true)]
    [InlineData(Visibility.Collapsed, "Hello")]
    public void Convert(Visibility expected, object? input)
        => Assert.Equal(
            expected,
            converter.Convert(input, targetType: null, parameter: null, culture: null));

    [Theory]
    [InlineData(Visibility.Visible, null, null)]
    [InlineData(Visibility.Collapsed, true, null)]
    [InlineData(Visibility.Collapsed, "Hello", null)]
    [InlineData(Visibility.Visible, null, Visibility.Collapsed)]
    [InlineData(Visibility.Collapsed, true, Visibility.Collapsed)]
    [InlineData(Visibility.Collapsed, "Hello", Visibility.Collapsed)]
    [InlineData(Visibility.Visible, null, Visibility.Hidden)]
    [InlineData(Visibility.Collapsed, true, Visibility.Hidden)]
    [InlineData(Visibility.Collapsed, "Hello", Visibility.Hidden)]
    [InlineData(Visibility.Visible, null, Visibility.Visible)]
    [InlineData(Visibility.Collapsed, true, Visibility.Visible)]
    [InlineData(Visibility.Collapsed, "Hello", Visibility.Visible)]
    public void Convert_Parameter(Visibility expected, object? input, object? parameter)
        => Assert.Equal(
            expected,
            converter.Convert(input, targetType: null, parameter, culture: null));

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