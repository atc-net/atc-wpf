namespace Atc.Wpf.Controls.Tests.Selectors;

public sealed class RenderFlagIndicatorTypeToVisibilityValueConverterTests
{
    private static readonly RenderFlagIndicatorTypeToVisibilityValueConverter Sut = new();

    [Theory]
    [InlineData(RenderFlagIndicatorType.None, RenderFlagIndicatorType.None)]
    [InlineData(RenderFlagIndicatorType.Flat16, RenderFlagIndicatorType.Flat16)]
    [InlineData(RenderFlagIndicatorType.Shiny16, RenderFlagIndicatorType.Shiny16)]
    public void Convert_returns_Visible_when_actual_matches_wanted(
        RenderFlagIndicatorType actual,
        RenderFlagIndicatorType wanted)
    {
        var result = Sut.Convert(
            actual,
            typeof(Visibility),
            wanted,
            CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Visible, result);
    }

    [Theory]
    [InlineData(RenderFlagIndicatorType.Flat16, RenderFlagIndicatorType.Shiny16)]
    [InlineData(RenderFlagIndicatorType.Shiny16, RenderFlagIndicatorType.None)]
    [InlineData(RenderFlagIndicatorType.None, RenderFlagIndicatorType.Flat16)]
    public void Convert_returns_Collapsed_when_actual_does_not_match_wanted(
        RenderFlagIndicatorType actual,
        RenderFlagIndicatorType wanted)
    {
        var result = Sut.Convert(
            actual,
            typeof(Visibility),
            wanted,
            CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_throws_when_value_is_null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Sut.Convert(
                value: null,
                typeof(Visibility),
                RenderFlagIndicatorType.None,
                CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Convert_throws_when_value_is_wrong_type()
    {
        Assert.ThrowsAny<Exception>(() =>
            Sut.Convert(
                "not an indicator",
                typeof(Visibility),
                RenderFlagIndicatorType.None,
                CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Convert_throws_when_parameter_is_wrong_type()
    {
        Assert.ThrowsAny<Exception>(() =>
            Sut.Convert(
                RenderFlagIndicatorType.None,
                typeof(Visibility),
                parameter: "not an indicator",
                CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertBack_throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            Sut.ConvertBack(
                Visibility.Visible,
                typeof(RenderFlagIndicatorType),
                parameter: null,
                CultureInfo.InvariantCulture));
    }
}