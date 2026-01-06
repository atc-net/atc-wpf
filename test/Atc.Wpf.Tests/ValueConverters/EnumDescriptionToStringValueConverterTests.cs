// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class EnumDescriptionToStringValueConverterTests
{
    private readonly IValueConverter converter = new EnumDescriptionToStringValueConverter();

    [Theory]
    [InlineData("Monday", DayOfWeek.Monday, null)]
    [InlineData("MONDAY", DayOfWeek.Monday, "U")]
    [InlineData("monday", DayOfWeek.Monday, "L")]
    [InlineData("Monday", DayOfWeek.Monday, "u")]
    [InlineData("monday", DayOfWeek.Monday, "l")]
    [InlineData("mONDAY", DayOfWeek.Monday, "Ul")]
    [InlineData("Monday", DayOfWeek.Monday, "Lu")]
    [InlineData("MONDAY.", DayOfWeek.Monday, "U.")]
    [InlineData("monday.", DayOfWeek.Monday, "L.")]
    [InlineData("Monday.", DayOfWeek.Monday, "u.")]
    [InlineData("monday.", DayOfWeek.Monday, "l.")]
    [InlineData("mONDAY.", DayOfWeek.Monday, "Ul.")]
    [InlineData("Monday.", DayOfWeek.Monday, "Lu.")]
    [InlineData("MONDAY:", DayOfWeek.Monday, "U:")]
    [InlineData("monday:", DayOfWeek.Monday, "L:")]
    [InlineData("Monday:", DayOfWeek.Monday, "u:")]
    [InlineData("monday:", DayOfWeek.Monday, "l:")]
    [InlineData("mONDAY:", DayOfWeek.Monday, "Ul:")]
    [InlineData("Monday:", DayOfWeek.Monday, "Lu:")]
    public void Convert(
        string expected,
        DayOfWeek input,
        string? inputParameter)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                inputParameter,
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