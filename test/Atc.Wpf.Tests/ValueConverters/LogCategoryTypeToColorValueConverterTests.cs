namespace Atc.Wpf.Tests.ValueConverters;

public sealed class LogCategoryTypeToColorValueConverterTests
{
    private readonly IValueConverter converter = new LogCategoryTypeToColorValueConverter();

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
    [InlineData(LogCategoryType.Critical)]
    [InlineData(LogCategoryType.Error)]
    [InlineData(LogCategoryType.Warning)]
    [InlineData(LogCategoryType.Security)]
    [InlineData(LogCategoryType.Audit)]
    [InlineData(LogCategoryType.Service)]
    [InlineData(LogCategoryType.UI)]
    [InlineData(LogCategoryType.Information)]
    [InlineData(LogCategoryType.Debug)]
    [InlineData(LogCategoryType.Trace)]
    public void Convert_LogCategoryType_Returns_Expected_Color(
        LogCategoryType input)
    {
        // Arrange
        var expected = input switch
        {
            LogCategoryType.Critical => Colors.Red,
            LogCategoryType.Error => Colors.Crimson,
            LogCategoryType.Warning => Colors.Goldenrod,
            LogCategoryType.Security => Colors.LightCyan,
            LogCategoryType.Audit => Colors.AntiqueWhite,
            LogCategoryType.Service => Colors.BurlyWood,
            LogCategoryType.UI => Colors.Aquamarine,
            LogCategoryType.Information => Colors.DodgerBlue,
            LogCategoryType.Debug => Colors.CadetBlue,
            LogCategoryType.Trace => Colors.Gray,
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