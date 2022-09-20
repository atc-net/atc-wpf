namespace Atc.Wpf.Tests.ValueConverters;

public class MultiBoolToVisibilityVisibleValueConverterTests
{
    private readonly IMultiValueConverter converter = new MultiBoolToVisibilityVisibleValueConverter();

    private readonly object[] inputSet1 = { true, true, true };
    private readonly object[] inputSet2 = { true, false, true };

    [Theory]
    [InlineData(Visibility.Visible, 1)]
    [InlineData(Visibility.Collapsed, 2)]
    public void Convert(Visibility expected, int inputSetNumber)
    {
        // Arrange
        var input = inputSetNumber switch
        {
            1 => inputSet1,
            2 => inputSet2,
            _ => Array.Empty<object>(),
        };

        // Atc
        var actual = converter.Convert(input, targetType: null, parameter: null, culture: null);

        // Arrange
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ConvertBack_Should_Throw_Exception()
    {
        // Act
        var exception = Record.Exception(() => converter.ConvertBack(value: null, targetTypes: null, parameter: null, culture: null));

        // Assert
        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}