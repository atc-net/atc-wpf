namespace Atc.Wpf.Tests.ValueConverters;

public sealed class IntegerToTimeSpanValueConverterTests
{
    private readonly IntegerToTimeSpanValueConverter sut = IntegerToTimeSpanValueConverter.Instance;

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1000, 1000)]
    [InlineData(60000, 60000)]
    [InlineData(3600000, 3600000)]
    public void Convert_WithValidInteger_ShouldReturnCorrectTimeSpan(
        int inputMs,
        double expectedMs)
    {
        // Act
        var result = sut.Convert(
            inputMs,
            typeof(TimeSpan),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        result
            .Should()
            .BeOfType<TimeSpan>();
        ((TimeSpan)result).TotalMilliseconds
            .Should()
            .Be(expectedMs);
    }

    [Fact]
    public void Convert_WithNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => sut.Convert(
            null,
            typeof(TimeSpan),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        act
            .Should()
            .Throw<ArgumentNullException>();
    }

    [Fact]
    public void Convert_WithInvalidType_ShouldThrowUnexpectedTypeException()
    {
        // Act
        var act = () => sut.Convert(
            "not an int",
            typeof(TimeSpan),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        act
            .Should()
            .Throw<UnexpectedTypeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1000)]
    [InlineData(60000)]
    [InlineData(3600000)]
    public void ConvertBack_WithValidTimeSpan_ShouldReturnCorrectInteger(
        int expectedMs)
    {
        // Arrange
        var timeSpan = TimeSpan.FromMilliseconds(expectedMs);

        // Act
        var result = sut.ConvertBack(
            timeSpan,
            typeof(int),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        result
            .Should()
            .BeOfType<int>();
        ((int)result)
            .Should()
            .Be(expectedMs);
    }

    [Fact]
    public void ConvertBack_WithNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => sut.ConvertBack(
            null,
            typeof(int),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        act
            .Should()
            .Throw<ArgumentNullException>();
    }

    [Fact]
    public void ConvertBack_WithInvalidType_ShouldThrowUnexpectedTypeException()
    {
        // Act
        var act = () => sut.ConvertBack(
            "not a timespan",
            typeof(int),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        act
            .Should()
            .Throw<UnexpectedTypeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1000)]
    [InlineData(60000)]
    [InlineData(3600000)]
    public void RoundTrip_ShouldPreserveValue(int inputMs)
    {
        // Act
        var timeSpanResult = sut.Convert(
            inputMs,
            typeof(TimeSpan),
            null,
            CultureInfo.InvariantCulture);
        var intResult = sut.ConvertBack(
            timeSpanResult,
            typeof(int),
            null,
            CultureInfo.InvariantCulture);

        // Assert
        intResult
            .Should()
            .Be(inputMs);
    }
}