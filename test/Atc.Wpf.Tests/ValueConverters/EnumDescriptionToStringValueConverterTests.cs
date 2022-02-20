namespace Atc.Wpf.Tests.ValueConverters;

public class EnumDescriptionToStringValueConverterTests
{
    private readonly IValueConverter converter = new EnumDescriptionToStringValueConverter();

    [Theory]
    [InlineData("Monday", DayOfWeek.Monday)]
    public void Convert(string expected, DayOfWeek input)
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