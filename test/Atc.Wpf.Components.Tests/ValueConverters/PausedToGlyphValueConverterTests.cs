// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Components.Tests.ValueConverters;

public sealed class PausedToGlyphValueConverterTests
{
    private readonly IValueConverter converter = new PausedToGlyphValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(PausedToGlyphValueConverter.Instance);

    [Fact]
    public void Convert_True_ReturnsPlayGlyph()
    {
        var actual = (string)converter.Convert(
            value: true,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.False(string.IsNullOrEmpty(actual));
        Assert.Equal("⏵", actual);
    }

    [Fact]
    public void Convert_False_ReturnsPauseGlyph()
    {
        var actual = (string)converter.Convert(
            value: false,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.False(string.IsNullOrEmpty(actual));
        Assert.Equal("⏸", actual);
    }

    [Fact]
    public void Convert_Null_ReturnsPauseGlyph()
        => Assert.Equal(
            "⏸",
            converter.Convert(
                value: null,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: "⏵",
            targetType: typeof(bool),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
    }
}