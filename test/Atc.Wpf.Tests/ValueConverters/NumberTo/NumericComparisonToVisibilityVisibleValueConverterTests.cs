// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class NumericComparisonToVisibilityVisibleValueConverterTests
{
    private readonly IValueConverter converter = new NumericComparisonToVisibilityVisibleValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(NumericComparisonToVisibilityVisibleValueConverter.Instance);

    [Theory]
    [InlineData(Visibility.Visible, 10, ">5")]
    [InlineData(Visibility.Collapsed, 5, ">5")]
    [InlineData(Visibility.Collapsed, 4, ">5")]
    [InlineData(Visibility.Visible, 5, ">=5")]
    [InlineData(Visibility.Visible, 10, ">=5")]
    [InlineData(Visibility.Collapsed, 4, ">=5")]
    [InlineData(Visibility.Visible, 4, "<5")]
    [InlineData(Visibility.Collapsed, 5, "<5")]
    [InlineData(Visibility.Visible, 5, "<=5")]
    [InlineData(Visibility.Visible, 4, "<=5")]
    [InlineData(Visibility.Collapsed, 6, "<=5")]
    [InlineData(Visibility.Visible, 5, "=5")]
    [InlineData(Visibility.Collapsed, 6, "=5")]
    [InlineData(Visibility.Visible, 6, "<>5")]
    [InlineData(Visibility.Collapsed, 5, "<>5")]
    public void Convert_OperatorExpressions(
        Visibility expected,
        int input,
        string parameter)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: parameter,
                culture: CultureInfo.InvariantCulture));

    [Theory]
    [InlineData(Visibility.Visible, 5, "between:1,10")]
    [InlineData(Visibility.Visible, 1, "between:1,10")]
    [InlineData(Visibility.Visible, 10, "between:1,10")]
    [InlineData(Visibility.Collapsed, 0, "between:1,10")]
    [InlineData(Visibility.Collapsed, 11, "between:1,10")]
    [InlineData(Visibility.Visible, 5, "between: 1 , 10 ")]
    public void Convert_BetweenExpressions(
        Visibility expected,
        int input,
        string parameter)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: parameter,
                culture: CultureInfo.InvariantCulture));

    [Theory]
    [InlineData(1.5, ">1")]
    [InlineData(99.99, ">=99")]
    public void Convert_AcceptsDouble(
        double input,
        string parameter)
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                input,
                targetType: null,
                parameter: parameter,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_AcceptsLong()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: 5_000_000_000L,
                targetType: null,
                parameter: ">100",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_AcceptsDecimal()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: 42.5m,
                targetType: null,
                parameter: ">42",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NullValue_ReturnsCollapsed()
        => Assert.Equal(
            Visibility.Collapsed,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: ">5",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NonNumericValue_ReturnsCollapsed()
        => Assert.Equal(
            Visibility.Collapsed,
            converter.Convert(
                value: "not a number",
                targetType: null,
                parameter: ">5",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NullParameter_ReturnsCollapsed()
        => Assert.Equal(
            Visibility.Collapsed,
            converter.Convert(
                value: 5,
                targetType: null,
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_MalformedParameter_Throws()
    {
        var exception = Record.Exception(() => converter.Convert(
            value: 5,
            targetType: null,
            parameter: "not-a-comparison",
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public void Convert_MalformedBetween_Throws()
    {
        var exception = Record.Exception(() => converter.Convert(
            value: 5,
            targetType: null,
            parameter: "between:bad",
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: Visibility.Visible,
            targetType: null,
            parameter: ">5",
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}