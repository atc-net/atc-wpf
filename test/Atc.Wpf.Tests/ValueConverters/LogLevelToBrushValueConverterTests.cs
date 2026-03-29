// ReSharper disable PossibleNullReferenceException
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class LogLevelToBrushValueConverterTests
{
    private readonly IValueConverter converter = new LogLevelToBrushValueConverter();

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
        Assert.NotNull(actual);
        var brush = Assert.IsType<SolidColorBrush>(actual);
        Assert.Equal(Brushes.DeepPink.Color, brush.Color);
    }

    [Theory]
    [InlineData(LogLevel.Critical, nameof(Brushes.Red))]
    [InlineData(LogLevel.Error, nameof(Brushes.Crimson))]
    [InlineData(LogLevel.Warning, nameof(Brushes.Goldenrod))]
    [InlineData(LogLevel.Information, nameof(Brushes.DodgerBlue))]
    [InlineData(LogLevel.Debug, nameof(Brushes.CadetBlue))]
    [InlineData(LogLevel.Trace, nameof(Brushes.Gray))]
    public void Convert_LogLevel_Returns_Expected_Brush(
        LogLevel input,
        string expectedBrushName)
    {
        // Arrange
        var expectedBrush = (SolidColorBrush)typeof(Brushes)
            .GetProperty(expectedBrushName)!
            .GetValue(null)!;

        // Act
        var actual = converter.Convert(
            input,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.NotNull(actual);
        var brush = Assert.IsType<SolidColorBrush>(actual);
        Assert.Equal(expectedBrush.Color, brush.Color);
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