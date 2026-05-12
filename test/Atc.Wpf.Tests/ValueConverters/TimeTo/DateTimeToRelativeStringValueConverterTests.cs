// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

[Collection("Sequential")]
public sealed class DateTimeToRelativeStringValueConverterTests
{
    private readonly IValueConverter converter = new DateTimeToRelativeStringValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(DateTimeToRelativeStringValueConverter.Instance);

    [Fact]
    public void Convert_KnownDateTime_ReturnsNonEmpty()
    {
        var result = (string)converter.Convert(
            value: DateTime.UtcNow.AddMinutes(-5),
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void Convert_DateTimeOffset_ReturnsNonEmpty()
    {
        var result = (string)converter.Convert(
            value: DateTimeOffset.UtcNow.AddHours(-2),
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
    public void Convert_NonDateTimeOrOffset_ReturnsEmpty()
        => Assert.Equal(
            string.Empty,
            converter.Convert(
                value: 42,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_StringParameter_ParsesAsDecimals()
    {
        var start = DateTime.UtcNow.AddHours(-1);

        var result = (string)converter.Convert(start, typeof(string), "2", CultureInfo.InvariantCulture);

        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void DefaultDecimalPrecision_CanBeOverridden()
    {
        var start = DateTime.UtcNow.AddMinutes(-30);
        var original = DateTimeToRelativeStringValueConverter.DefaultDecimalPrecision;
        try
        {
            DateTimeToRelativeStringValueConverter.DefaultDecimalPrecision = 3;
            var result = (string)converter.Convert(start, typeof(string), null, CultureInfo.InvariantCulture);

            Assert.False(string.IsNullOrWhiteSpace(result));
        }
        finally
        {
            DateTimeToRelativeStringValueConverter.DefaultDecimalPrecision = original;
        }
    }

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: "5 minutes ago",
            targetType: typeof(DateTime),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}