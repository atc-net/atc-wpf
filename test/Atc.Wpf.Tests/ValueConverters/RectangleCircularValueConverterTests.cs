namespace Atc.Wpf.Tests.ValueConverters;

public sealed class RectangleCircularValueConverterTests
{
    [Theory]
    [InlineData(100d, 100d, 50d)]
    [InlineData(200d, 100d, 50d)]
    [InlineData(100d, 200d, 50d)]
    [InlineData(40d, 80d, 20d)]
    public void Convert_returns_half_of_the_smaller_dimension(
        double width,
        double height,
        double expected)
    {
        var actual = RectangleCircularValueConverter.Instance.Convert(
            new object[] { width, height },
            typeof(double),
            parameter: null!,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0d, 100d)]
    [InlineData(100d, 0d)]
    [InlineData(0d, 0d)]
    public void Convert_returns_zero_when_either_dimension_is_zero(
        double width,
        double height)
    {
        var actual = RectangleCircularValueConverter.Instance.Convert(
            new object[] { width, height },
            typeof(double),
            parameter: null!,
            CultureInfo.InvariantCulture);

        Assert.Equal(0d, actual);
    }

    [Fact]
    public void Convert_returns_UnsetValue_when_values_array_has_wrong_length()
    {
        var actual = RectangleCircularValueConverter.Instance.Convert(
            new object[] { 100d },
            typeof(double),
            parameter: null!,
            CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, actual);
    }

    [Fact]
    public void Convert_returns_UnsetValue_when_values_are_not_doubles()
    {
        var actual = RectangleCircularValueConverter.Instance.Convert(
            new object[] { "100", "100" },
            typeof(double),
            parameter: null!,
            CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, actual);
    }

    [Fact]
    public void ConvertBack_throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            RectangleCircularValueConverter.Instance.ConvertBack(
                50d,
                [typeof(double), typeof(double)],
                parameter: null!,
                CultureInfo.InvariantCulture));
    }
}