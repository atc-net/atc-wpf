namespace Atc.Wpf.Tests.ValueConverters;

public sealed class IsNullValueConverterTests
{
    private readonly IValueConverter converter = new IsNullValueConverter();

    [Theory]
    [InlineData(true, null)]
    [InlineData(false, "hello")]
    [InlineData(false, 42)]
    public void Convert_Returns_Expected(
        bool expected,
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