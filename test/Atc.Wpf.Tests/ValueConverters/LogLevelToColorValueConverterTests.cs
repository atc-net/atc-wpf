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

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(LogLevelToColorValueConverter.Instance);

    [Fact]
    public void SetColor_OverridesGetColor()
    {
        try
        {
            LogLevelToColorValueConverter.SetColor(LogLevel.Warning, Colors.Orange);
            Assert.Equal(Colors.Orange, LogLevelToColorValueConverter.GetColor(LogLevel.Warning));
        }
        finally
        {
            LogLevelToColorValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void OverrideProperty_PropagatesToConvert()
    {
        try
        {
            LogLevelToColorValueConverter.WarningColor = Colors.Orange;

            var actual = converter.Convert(
                LogLevel.Warning,
                targetType: null,
                parameter: null,
                culture: null);

            Assert.Equal(Colors.Orange, actual);
        }
        finally
        {
            LogLevelToColorValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void OverrideColor_PropagatesToBrushConverter()
    {
        try
        {
            LogLevelToColorValueConverter.WarningColor = Colors.Orange;

            var brush = LogLevelToBrushValueConverter.GetBrush(LogLevel.Warning);

            Assert.Equal(Colors.Orange, brush.Color);
            Assert.True(brush.IsFrozen);
        }
        finally
        {
            LogLevelToColorValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void ResetToDefaults_RestoresOriginalPalette()
    {
        LogLevelToColorValueConverter.WarningColor = Colors.Orange;
        LogLevelToColorValueConverter.CriticalColor = Colors.Black;

        LogLevelToColorValueConverter.ResetToDefaults();

        Assert.Equal(LogLevelToColorValueConverter.DefaultWarningColor, LogLevelToColorValueConverter.WarningColor);
        Assert.Equal(LogLevelToColorValueConverter.DefaultCriticalColor, LogLevelToColorValueConverter.CriticalColor);
    }

    [Fact]
    public void LogLevelNone_ReturnsFallbackColor()
    {
        try
        {
            LogLevelToColorValueConverter.FallbackColor = Colors.Magenta;

            var actual = converter.Convert(
                LogLevel.None,
                targetType: null,
                parameter: null,
                culture: null);

            Assert.Equal(Colors.Magenta, actual);
        }
        finally
        {
            LogLevelToColorValueConverter.ResetToDefaults();
        }
    }
}