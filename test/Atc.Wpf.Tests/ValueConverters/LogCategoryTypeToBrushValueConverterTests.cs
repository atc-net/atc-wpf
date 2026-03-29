// ReSharper disable PossibleNullReferenceException
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class LogCategoryTypeToBrushValueConverterTests
{
    private readonly IValueConverter converter = new LogCategoryTypeToBrushValueConverter();

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
    [InlineData(LogCategoryType.Critical, nameof(Brushes.Red))]
    [InlineData(LogCategoryType.Error, nameof(Brushes.Crimson))]
    [InlineData(LogCategoryType.Warning, nameof(Brushes.Goldenrod))]
    [InlineData(LogCategoryType.Security, nameof(Brushes.LightCyan))]
    [InlineData(LogCategoryType.Audit, nameof(Brushes.AntiqueWhite))]
    [InlineData(LogCategoryType.Service, nameof(Brushes.BurlyWood))]
    [InlineData(LogCategoryType.UI, nameof(Brushes.Aquamarine))]
    [InlineData(LogCategoryType.Information, nameof(Brushes.DodgerBlue))]
    [InlineData(LogCategoryType.Debug, nameof(Brushes.CadetBlue))]
    [InlineData(LogCategoryType.Trace, nameof(Brushes.Gray))]
    public void Convert_LogCategoryType_Returns_Expected_Brush(
        LogCategoryType input,
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