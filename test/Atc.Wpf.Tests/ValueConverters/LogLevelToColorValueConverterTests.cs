namespace Atc.Wpf.Tests.ValueConverters;

public sealed class LogLevelToColorValueConverterTests
{
    private readonly IValueConverter converter = new LogLevelToColorValueConverter();

    [Fact]
    public void Convert_Null_Returns_DeepPink()
    {
        // Act
        var actual = converter.Convert(
            value: null,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.Equal(Colors.DeepPink, actual);
    }

    [Theory]
    [InlineData(LogLevel.Critical)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Trace)]
    public void Convert_LogLevel_Returns_Expected_Color(LogLevel input)
    {
        // Arrange
        var expected = input switch
        {
            LogLevel.Critical => Colors.Red,
            LogLevel.Error => Colors.Crimson,
            LogLevel.Warning => Colors.Goldenrod,
            LogLevel.Information => Colors.DodgerBlue,
            LogLevel.Debug => Colors.CadetBlue,
            LogLevel.Trace => Colors.Gray,
            _ => throw new ArgumentOutOfRangeException(nameof(input)),
        };

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
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}