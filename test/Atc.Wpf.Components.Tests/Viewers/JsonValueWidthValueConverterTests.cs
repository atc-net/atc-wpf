namespace Atc.Wpf.Components.Tests.Viewers;

public sealed class JsonValueWidthValueConverterTests
{
    [Theory]
    [InlineData(800d, 250, 550d)]
    [InlineData(0d, 250, double.NaN)]
    [InlineData(100d, 250, double.NaN)]
    public void Convert_never_returns_a_negative_width(
        double viewerWidth,
        int reserved,
        double expected)
    {
        var result = JsonValueWidthValueConverter.Instance.Convert(
            [viewerWidth, reserved.ToString(CultureInfo.InvariantCulture)],
            typeof(double),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, (double)result);
    }

    [Fact]
    public void Convert_without_a_width_lets_the_text_size_itself()
    {
        var result = JsonValueWidthValueConverter.Instance.Convert(
            [DependencyProperty.UnsetValue, "250"],
            typeof(double),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(double.NaN, (double)result);
    }
}