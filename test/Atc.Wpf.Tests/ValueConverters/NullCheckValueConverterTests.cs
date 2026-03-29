namespace Atc.Wpf.Tests.ValueConverters;

public sealed class NullCheckValueConverterTests
{
    private readonly IValueConverter converter = new NullCheckValueConverter();

    [Fact]
    public void Convert_Null_Value_Returns_Parameter()
    {
        // Act
        var actual = converter.Convert(
            value: null,
            targetType: null,
            parameter: "fallback",
            culture: null);

        // Assert
        Assert.Equal("fallback", actual);
    }

    [Fact]
    public void Convert_NonNull_Value_Returns_Value()
    {
        // Act
        var actual = converter.Convert(
            value: "value",
            targetType: null,
            parameter: "fallback",
            culture: null);

        // Assert
        Assert.Equal("value", actual);
    }

    [Fact]
    public void Convert_NonNull_Value_With_Null_Parameter_Returns_Value()
    {
        // Act
        var actual = converter.Convert(
            value: 42,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.Equal(42, actual);
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