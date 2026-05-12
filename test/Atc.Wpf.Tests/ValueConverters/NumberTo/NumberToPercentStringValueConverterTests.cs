// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class NumberToPercentStringValueConverterTests
{
    private readonly IValueConverter converter = new NumberToPercentStringValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(NumberToPercentStringValueConverter.Instance);

    [Fact]
    public void Convert_DefaultDecimals_NoFractional()
    {
        var result = (string)converter.Convert(
            value: 0.42,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Equal(0.42.ToString("P0", CultureInfo.InvariantCulture), result);
    }

    [Fact]
    public void Convert_WithDecimalsParameter()
    {
        var result = (string)converter.Convert(
            value: 0.1234,
            targetType: typeof(string),
            parameter: 2,
            culture: CultureInfo.InvariantCulture);

        Assert.Equal(0.1234.ToString("P2", CultureInfo.InvariantCulture), result);
    }

    [Fact]
    public void Convert_StringParameter_ParsesAsDecimals()
    {
        var result = (string)converter.Convert(
            value: 0.5,
            targetType: typeof(string),
            parameter: "1",
            culture: CultureInfo.InvariantCulture);

        Assert.Equal(0.5.ToString("P1", CultureInfo.InvariantCulture), result);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(42L)]
    [InlineData(42.0f)]
    public void Convert_AcceptsNumericTypes(object value)
    {
        var result = (string)converter.Convert(
            value,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void Convert_AcceptsDecimal()
    {
        var result = (string)converter.Convert(
            value: 0.5m,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Equal(0.5.ToString("P0", CultureInfo.InvariantCulture), result);
    }

    [Fact]
    public void Convert_Null_ReturnsEmpty()
        => Assert.Equal(
            string.Empty,
            converter.Convert(
                value: null,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NonNumeric_ReturnsEmpty()
        => Assert.Equal(
            string.Empty,
            converter.Convert(
                value: "not a number",
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Theory]
    [InlineData("42%", 0.42)]
    [InlineData("42 %", 0.42)]
    [InlineData("42", 0.42)]
    [InlineData("  42 %  ", 0.42)]
    public void ConvertBack_ParsesPercentString(
        string input,
        double expected)
    {
        var result = converter.ConvertBack(
            input,
            targetType: typeof(double),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        var actual = Assert.IsType<double>(result);
        Assert.Equal(expected, actual, precision: 10);
    }

    [Fact]
    public void ConvertBack_NullOrEmpty_ReturnsDoNothing()
        => Assert.Same(
            Binding.DoNothing,
            converter.ConvertBack(
                value: null,
                targetType: typeof(double),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void ConvertBack_Unparseable_ReturnsDoNothing()
        => Assert.Same(
            Binding.DoNothing,
            converter.ConvertBack(
                value: "not a number",
                targetType: typeof(double),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void RoundTrip_StaysClose()
    {
        const double original = 0.4242;
        var asText = (string)converter.Convert(
            original,
            typeof(string),
            parameter: 2,
            culture: CultureInfo.InvariantCulture);
        var parsed = (double)converter.ConvertBack(
            asText,
            typeof(double),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Equal(original, parsed, precision: 4);
    }
}