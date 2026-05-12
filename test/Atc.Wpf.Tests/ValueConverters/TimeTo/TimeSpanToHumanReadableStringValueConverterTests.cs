// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

[Collection("Sequential")]
public sealed class TimeSpanToHumanReadableStringValueConverterTests
{
    private readonly IValueConverter converter = new TimeSpanToHumanReadableStringValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(TimeSpanToHumanReadableStringValueConverter.Instance);

    [Fact]
    public void Convert_KnownTimeSpan_ReturnsNonEmpty()
    {
        var result = (string)converter.Convert(
            value: TimeSpan.FromMinutes(75),
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
    public void Convert_NonTimeSpan_ReturnsEmpty()
        => Assert.Equal(
            string.Empty,
            converter.Convert(
                value: 42,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_RespectsParameterDecimalPrecision()
    {
        var ts = TimeSpan.FromHours(1.5);

        var oneDecimal = (string)converter.Convert(ts, typeof(string), 1, CultureInfo.InvariantCulture);
        var threeDecimals = (string)converter.Convert(ts, typeof(string), 3, CultureInfo.InvariantCulture);

        Assert.Equal(ts.GetPrettyTime(1), oneDecimal);
        Assert.Equal(ts.GetPrettyTime(3), threeDecimals);
    }

    [Fact]
    public void Convert_StringParameter_ParsesAsDecimals()
    {
        var ts = TimeSpan.FromHours(1.5);

        var result = (string)converter.Convert(ts, typeof(string), "2", CultureInfo.InvariantCulture);

        Assert.Equal(ts.GetPrettyTime(2), result);
    }

    [Fact]
    public void Convert_InvalidStringParameter_FallsBackToDefault()
    {
        var ts = TimeSpan.FromMinutes(30);

        var result = (string)converter.Convert(ts, typeof(string), "not-a-number", CultureInfo.InvariantCulture);

        Assert.Equal(ts.GetPrettyTime(TimeSpanToHumanReadableStringValueConverter.DefaultDecimalPrecision), result);
    }

    [Fact]
    public void DefaultDecimalPrecision_CanBeOverridden()
    {
        var ts = TimeSpan.FromMinutes(30);
        var original = TimeSpanToHumanReadableStringValueConverter.DefaultDecimalPrecision;
        try
        {
            TimeSpanToHumanReadableStringValueConverter.DefaultDecimalPrecision = 4;

            var result = (string)converter.Convert(ts, typeof(string), null, CultureInfo.InvariantCulture);

            Assert.Equal(ts.GetPrettyTime(4), result);
        }
        finally
        {
            TimeSpanToHumanReadableStringValueConverter.DefaultDecimalPrecision = original;
        }
    }

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: "1h 15m",
            targetType: typeof(TimeSpan),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}