namespace Atc.Wpf.Tests.ValueConverters;

public class ObjectNotNullToVisibilityVisibleValueConverterTests
{
    private readonly IValueConverter converter = new ObjectNotNullToVisibilityVisibleValueConverter();

    [Theory]
    [InlineData(Visibility.Collapsed, null)]
    [InlineData(Visibility.Visible, true)]
    [InlineData(Visibility.Visible, "Hello")]
    public void Convert(Visibility expected, object? input)
        => Assert.Equal(
            expected,
            converter.Convert(input, targetType: null, parameter: null, culture: null));

    [Theory]
    [InlineData(Visibility.Collapsed, null, null)]
    [InlineData(Visibility.Visible, true, null)]
    [InlineData(Visibility.Visible, "Hello", null)]
    [InlineData(Visibility.Collapsed, null, Visibility.Collapsed)]
    [InlineData(Visibility.Visible, true, Visibility.Collapsed)]
    [InlineData(Visibility.Visible, "Hello", Visibility.Collapsed)]
    [InlineData(Visibility.Hidden, null, Visibility.Hidden)]
    [InlineData(Visibility.Visible, true, Visibility.Hidden)]
    [InlineData(Visibility.Visible, "Hello", Visibility.Hidden)]
    [InlineData(Visibility.Collapsed, null, Visibility.Visible)]
    [InlineData(Visibility.Visible, true, Visibility.Visible)]
    [InlineData(Visibility.Visible, "Hello", Visibility.Visible)]
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