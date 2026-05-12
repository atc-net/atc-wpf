// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class NumericComparisonToVisibilityCollapsedValueConverterTests
{
    private readonly IValueConverter converter = new NumericComparisonToVisibilityCollapsedValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(NumericComparisonToVisibilityCollapsedValueConverter.Instance);

    [Theory]
    [InlineData(Visibility.Collapsed, 10, ">5")]
    [InlineData(Visibility.Visible, 5, ">5")]
    [InlineData(Visibility.Visible, 4, ">=5")]
    [InlineData(Visibility.Collapsed, 5, "=5")]
    [InlineData(Visibility.Visible, 5, "<>5")]
    [InlineData(Visibility.Collapsed, 6, "<>5")]
    [InlineData(Visibility.Collapsed, 5, "between:1,10")]
    [InlineData(Visibility.Visible, 11, "between:1,10")]
    public void Convert(
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

    [Fact]
    public void Convert_NullValue_ReturnsVisible()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: ">5",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
        => Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(
                Visibility.Visible,
                targetType: null,
                parameter: ">5",
                culture: CultureInfo.InvariantCulture));
}