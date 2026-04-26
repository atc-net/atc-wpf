namespace Atc.Wpf.Theming.Tests.Controls.Selectors;

public sealed class RenderColorIndicatorTypeToVisibilityValueConverterTests
{
    [Theory]
    [InlineData(RenderColorIndicatorType.None, RenderColorIndicatorType.None)]
    [InlineData(RenderColorIndicatorType.Circle, RenderColorIndicatorType.Circle)]
    [InlineData(RenderColorIndicatorType.Square, RenderColorIndicatorType.Square)]
    public void Convert_returns_Visible_when_actual_matches_wanted(
        RenderColorIndicatorType actual,
        RenderColorIndicatorType wanted)
    {
        var result = RenderColorIndicatorTypeToVisibilityValueConverter.Instance.Convert(
            actual,
            typeof(Visibility),
            wanted,
            CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Visible, result);
    }

    [Theory]
    [InlineData(RenderColorIndicatorType.Circle, RenderColorIndicatorType.Square)]
    [InlineData(RenderColorIndicatorType.Square, RenderColorIndicatorType.None)]
    [InlineData(RenderColorIndicatorType.None, RenderColorIndicatorType.Circle)]
    public void Convert_returns_Collapsed_when_actual_does_not_match_wanted(
        RenderColorIndicatorType actual,
        RenderColorIndicatorType wanted)
    {
        var result = RenderColorIndicatorTypeToVisibilityValueConverter.Instance.Convert(
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
            RenderColorIndicatorTypeToVisibilityValueConverter.Instance.Convert(
                value: null,
                typeof(Visibility),
                RenderColorIndicatorType.None,
                CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Convert_throws_UnexpectedTypeException_when_value_is_wrong_type()
    {
        Assert.ThrowsAny<Exception>(() =>
            RenderColorIndicatorTypeToVisibilityValueConverter.Instance.Convert(
                "not an indicator type",
                typeof(Visibility),
                RenderColorIndicatorType.None,
                CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertBack_throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            RenderColorIndicatorTypeToVisibilityValueConverter.Instance.ConvertBack(
                Visibility.Visible,
                typeof(RenderColorIndicatorType),
                parameter: null,
                CultureInfo.InvariantCulture));
    }
}