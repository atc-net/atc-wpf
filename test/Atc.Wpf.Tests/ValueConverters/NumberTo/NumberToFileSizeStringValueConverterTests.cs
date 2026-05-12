// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

[Collection("Sequential")]
public sealed class NumberToFileSizeStringValueConverterTests
{
    private readonly IValueConverter converter = new NumberToFileSizeStringValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(NumberToFileSizeStringValueConverter.Instance);

    [Fact]
    public void Formatter_Defaults_To_ByteSizeFormatterDefault()
        => Assert.Same(
            Atc.Units.DigitalInformation.ByteSizeFormatter.Default,
            NumberToFileSizeStringValueConverter.Formatter);

    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(1023L)]
    [InlineData(1024L)]
    [InlineData(1_572_864L)]
    [InlineData(1_073_741_824L)]
    public void Convert_KnownLongs_ReturnsNonEmptyString(long bytes)
    {
        var result = converter.Convert(
            value: bytes,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        var text = Assert.IsType<string>(result);
        Assert.False(string.IsNullOrWhiteSpace(text));
    }

    [Fact]
    public void Convert_IntValue_Works()
    {
        var result = (string)converter.Convert(
            value: 1024,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void Convert_DoubleValue_TruncatesToLong()
    {
        var result = (string)converter.Convert(
            value: 1024.5,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void Convert_DecimalValue_TruncatesToLong()
    {
        var result = (string)converter.Convert(
            value: 2048m,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.False(string.IsNullOrWhiteSpace(result));
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

    [Fact]
    public void Convert_RespectsCustomFormatter()
    {
        var custom = new Atc.Units.DigitalInformation.ByteSizeFormatter
        {
            NumberOfDecimals = 3,
        };
        var original = NumberToFileSizeStringValueConverter.Formatter;
        try
        {
            NumberToFileSizeStringValueConverter.Formatter = custom;

            var result = (string)converter.Convert(
                value: 1_572_864L,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture);

            Assert.Equal(custom.Format(1_572_864L), result);
        }
        finally
        {
            NumberToFileSizeStringValueConverter.Formatter = original;
        }
    }

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: "1 MB",
            targetType: typeof(long),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}