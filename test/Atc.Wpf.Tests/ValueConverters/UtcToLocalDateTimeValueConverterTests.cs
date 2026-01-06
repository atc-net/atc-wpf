namespace Atc.Wpf.Tests.ValueConverters;

public sealed class UtcToLocalDateTimeValueConverterTests
{
    private readonly UtcToLocalDateTimeValueConverter sut = UtcToLocalDateTimeValueConverter.Instance;

    [Fact]
    public void Convert_WithDateTimeOffset_ShouldReturnLocalTime()
    {
        // Arrange
        var utcDateTime = new DateTime(
            2024,
            7,
            15,
            12,
            30,
            45,
            DateTimeKind.Utc);
        var utcOffset = new DateTimeOffset(utcDateTime);

        // Act
        var result = sut.Convert(
            utcOffset,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        var expectedLocal = utcOffset
            .ToLocalTime()
            .DateTime;
        result
            .Should()
            .BeOfType<DateTime>()
            .And
            .Match<DateTime>(d => d.Kind == DateTimeKind.Unspecified || d.Kind == DateTimeKind.Local)
            .And
            .Be(expectedLocal);
    }

    [Fact]
    public void Convert_WithUtcDateTime_ShouldReturnLocalTime()
    {
        // Arrange
        var utcDateTime = new DateTime(
            2024,
            7,
            15,
            12,
            30,
            45,
            DateTimeKind.Utc);

        // Act
        var result = sut.Convert(
            utcDateTime,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        var expectedLocal = utcDateTime.ToLocalTime();
        result
            .Should()
            .BeOfType<DateTime>()
            .And
            .Match<DateTime>(d => d.Kind == DateTimeKind.Local)
            .And
            .Be(expectedLocal);
    }

    [Fact]
    public void Convert_WithLocalDateTime_ShouldReturnSameInstance()
    {
        // Arrange
        var localTime = new DateTime(
            2024,
            7,
            15,
            14,
            30,
            45,
            DateTimeKind.Local);

        // Act
        var result = sut.Convert(
            localTime,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        result
            .Should()
            .BeOfType<DateTime>()
            .And
            .Be(localTime);
    }

    [Fact]
    public void Convert_WithUnspecifiedDateTime_ShouldAssumeUtcAndConvert()
    {
        // Arrange
        var baseUtcTime = new DateTime(
            2024,
            7,
            15,
            12,
            30,
            45,
            DateTimeKind.Utc);
        var unspecified = DateTime.SpecifyKind(
            baseUtcTime,
            DateTimeKind.Unspecified);

        // Act
        var result = sut.Convert(
            unspecified,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        var expected = DateTime
            .SpecifyKind(
                unspecified,
                DateTimeKind.Utc)
            .ToLocalTime();
        result
            .Should()
            .BeOfType<DateTime>()
            .And
            .Be(expected);
    }

    [Fact]
    public void Convert_WithNull_ShouldReturnNull()
    {
        // Act
        var result = sut.Convert(
            null,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        result
            .Should()
            .BeNull();
    }

    [Fact]
    public void Convert_WithInvalidType_ShouldReturnNull()
    {
        // Act
        var result = sut.Convert(
            "not a date",
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        result
            .Should()
            .BeNull();
    }

    [Fact]
    public void ConvertBack_WithLocalDateTime_ShouldReturnUtcTime()
    {
        // Arrange
        var localDateTime = new DateTime(
            2024,
            7,
            15,
            14,
            30,
            45,
            DateTimeKind.Local);

        // Act
        var result = sut.ConvertBack(
            localDateTime,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        var expectedUtc = localDateTime.ToUniversalTime();
        result
            .Should()
            .BeOfType<DateTime>()
            .And
            .Match<DateTime>(d => d.Kind == DateTimeKind.Utc)
            .And
            .Be(expectedUtc);
    }

    [Fact]
    public void ConvertBack_WithUtcDateTime_ShouldReturnSameValue()
    {
        // Arrange
        var utcDateTime = new DateTime(
            2024,
            7,
            15,
            12,
            30,
            45,
            DateTimeKind.Utc);

        // Act
        var result = sut.ConvertBack(
            utcDateTime,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        result
            .Should()
            .BeOfType<DateTime>()
            .And
            .Be(utcDateTime);
    }

    [Fact]
    public void ConvertBack_WithUnspecifiedDateTime_ShouldAssumeLocalAndConvert()
    {
        // Arrange
        var unspecified = new DateTime(
            2024,
            7,
            15,
            14,
            30,
            45,
            DateTimeKind.Unspecified);

        // Act
        var result = sut.ConvertBack(
            unspecified,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        var expected = DateTime
            .SpecifyKind(
                unspecified,
                DateTimeKind.Local)
            .ToUniversalTime();
        result
            .Should()
            .BeOfType<DateTime>()
            .And
            .Be(expected);
    }

    [Fact]
    public void ConvertBack_WithNull_ShouldReturnNull()
    {
        // Act
        var result = sut.ConvertBack(
            null,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        result
            .Should()
            .BeNull();
    }

    [Fact]
    public void ConvertBack_WithInvalidType_ShouldReturnNull()
    {
        // Act
        var result = sut.ConvertBack(
            "not a date",
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        result
            .Should()
            .BeNull();
    }

    [Fact]
    public void RoundTrip_ShouldPreserveValue()
    {
        // Arrange
        var utcDateTime = new DateTime(
            2024,
            7,
            15,
            12,
            30,
            45,
            DateTimeKind.Utc);

        // Act
        var localResult = sut.Convert(
            utcDateTime,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);
        var utcResult = sut.ConvertBack(
            localResult,
            typeof(DateTime),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        utcResult
            .Should()
            .BeOfType<DateTime>()
            .And
            .Be(utcDateTime);
    }
}