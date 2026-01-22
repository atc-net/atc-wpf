// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class RegexValidationValueConverterTests
{
    private readonly IValueConverter converter = new RegexValidationValueConverter();

    [Theory]
    [InlineData(true, "test@example.com", @"^[\w.-]+@[\w.-]+\.\w+$")]
    [InlineData(true, "john.doe@company.org", @"^[\w.-]+@[\w.-]+\.\w+$")]
    [InlineData(false, "invalid-email", @"^[\w.-]+@[\w.-]+\.\w+$")]
    [InlineData(false, "@example.com", @"^[\w.-]+@[\w.-]+\.\w+$")]
    public void Convert_Email(
        bool expected,
        string input,
        string pattern)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: pattern,
                culture: null));

    [Theory]
    [InlineData(true, "123-456-7890", @"^\d{3}-\d{3}-\d{4}$")]
    [InlineData(false, "1234567890", @"^\d{3}-\d{3}-\d{4}$")]
    [InlineData(false, "abc-def-ghij", @"^\d{3}-\d{3}-\d{4}$")]
    public void Convert_PhoneNumber(
        bool expected,
        string input,
        string pattern)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: pattern,
                culture: null));

    [Theory]
    [InlineData(true, "ABC123", @"^[A-Z]{3}\d{3}$")]
    [InlineData(false, "abc123", @"^[A-Z]{3}\d{3}$")]
    [InlineData(false, "ABCD123", @"^[A-Z]{3}\d{3}$")]
    public void Convert_AlphanumericPattern(
        bool expected,
        string input,
        string pattern)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: pattern,
                culture: null));

    [Fact]
    public void Convert_WithNullValue_ReturnsFalse()
    {
        var result = converter.Convert(value: null, targetType: null, parameter: @"^\w+$", culture: null);

        Assert.False((bool)result);
    }

    [Fact]
    public void Convert_WithEmptyValue_ReturnsFalse()
    {
        var result = converter.Convert(value: string.Empty, targetType: null, parameter: @"^\w+$", culture: null);

        Assert.False((bool)result);
    }

    [Fact]
    public void Convert_WithNullPattern_ReturnsFalse()
    {
        var result = converter.Convert(value: "test", targetType: null, parameter: null, culture: null);

        Assert.False((bool)result);
    }

    [Fact]
    public void Convert_WithEmptyPattern_ReturnsFalse()
    {
        var result = converter.Convert(value: "test", targetType: null, parameter: string.Empty, culture: null);

        Assert.False((bool)result);
    }

    [Fact]
    public void Convert_WithInvalidRegexPattern_ReturnsFalse()
    {
        var result = converter.Convert(value: "test", targetType: null, parameter: "[invalid(regex", culture: null);

        Assert.False((bool)result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
        => Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(
                value: true,
                targetType: null,
                parameter: null,
                culture: null));
}