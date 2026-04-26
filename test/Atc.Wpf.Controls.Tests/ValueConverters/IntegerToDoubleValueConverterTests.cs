namespace Atc.Wpf.Controls.Tests.ValueConverters;

public sealed class IntegerToDoubleValueConverterTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-7)]
    [InlineData(int.MaxValue)]
    public void Convert_passes_int_input_through_unchanged(int input)
    {
        // The converter's switch arm is `int i => i`; with no implicit common
        // numeric type across all arms (int / decimal / double), each branch
        // boxes its own type. So an int stays an int.
        var actual = IntegerToDoubleValueConverter.Instance.Convert(
            input,
            typeof(double),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(input, Assert.IsType<int>(actual));
    }

    [Fact]
    public void Convert_passes_decimal_input_through_unchanged()
    {
        var actual = IntegerToDoubleValueConverter.Instance.Convert(
            12.34m,
            typeof(double),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(12.34m, Assert.IsType<decimal>(actual));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("text")]
    [InlineData(true)]
    public void Convert_returns_double_zero_for_unsupported_input(object? input)
    {
        var actual = IntegerToDoubleValueConverter.Instance.Convert(
            input,
            typeof(double),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(0d, Assert.IsType<double>(actual));
    }

    [Theory]
    [InlineData(0d)]
    [InlineData(3.14d)]
    [InlineData(-2.5d)]
    public void ConvertBack_passes_double_input_through_unchanged(double input)
    {
        var actual = IntegerToDoubleValueConverter.Instance.ConvertBack(
            input,
            typeof(int),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(input, Assert.IsType<double>(actual));
    }

    [Fact]
    public void ConvertBack_returns_double_zero_for_non_double_input()
    {
        // The fallback `_ => 0` is promoted to `0d` because the sibling arm is
        // `double d => d` and int → double is implicit, so the switch's common
        // type is double.
        var actual = IntegerToDoubleValueConverter.Instance.ConvertBack(
            "not a double",
            typeof(int),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(0d, Assert.IsType<double>(actual));
    }
}