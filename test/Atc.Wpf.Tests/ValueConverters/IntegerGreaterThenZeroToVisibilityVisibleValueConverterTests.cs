namespace Atc.Wpf.Tests.ValueConverters;

public sealed class IntegerGreaterThenZeroToVisibilityVisibleValueConverterTests
{
    private readonly IValueConverter converter = new IntegerGreaterThenZeroToVisibilityVisibleValueConverter();

    [Theory]
    [InlineData(Visibility.Collapsed, null)]
    [InlineData(Visibility.Collapsed, 0)]
    [InlineData(Visibility.Visible, 1)]
    [InlineData(Visibility.Visible, 5)]
    [InlineData(Visibility.Collapsed, -1)]
    public void Convert_Returns_Expected_Visibility(
        Visibility expected,
        object? input)
    {
        // Act
        var actual = converter.Convert(
            input,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: null,
            targetType: null,
            parameter: null,
            culture: null));

        Assert.IsType<NotSupportedException>(exception);
    }
}