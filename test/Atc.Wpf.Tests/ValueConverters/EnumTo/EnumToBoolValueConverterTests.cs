// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class EnumToBoolValueConverterTests
{
    private readonly IValueConverter converter = new EnumToBoolValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(EnumToBoolValueConverter.Instance);

    [Theory]
    [InlineData(true, DayOfWeek.Monday, "Monday")]
    [InlineData(true, DayOfWeek.Friday, "Friday")]
    [InlineData(true, DayOfWeek.Monday, "monday")]
    [InlineData(false, DayOfWeek.Monday, "Tuesday")]
    [InlineData(false, DayOfWeek.Monday, "")]
    [InlineData(false, DayOfWeek.Monday, null)]
    public void Convert(
        bool expected,
        DayOfWeek input,
        string? parameter)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: parameter,
                culture: null));

    [Fact]
    public void Convert_WithNullValue_ReturnsFalse()
        => Assert.False(
            (bool)converter.Convert(
                value: null,
                targetType: null,
                parameter: "Monday",
                culture: null));

    [Fact]
    public void Convert_WithNonEnumValue_ReturnsFalse()
        => Assert.False(
            (bool)converter.Convert(
                value: "NotAnEnum",
                targetType: null,
                parameter: "Monday",
                culture: null));

    [Fact]
    public void Convert_WithEnumParameter_ReturnsTrue()
        => Assert.True(
            (bool)converter.Convert(
                value: DayOfWeek.Monday,
                targetType: null,
                parameter: DayOfWeek.Monday,
                culture: null));

    [Fact]
    public void Convert_WithDifferentEnumParameter_ReturnsFalse()
        => Assert.False(
            (bool)converter.Convert(
                value: DayOfWeek.Monday,
                targetType: null,
                parameter: DayOfWeek.Tuesday,
                culture: null));

    [Theory]
    [InlineData(true, DayOfWeek.Monday, "Monday,Tuesday")]
    [InlineData(true, DayOfWeek.Tuesday, "Monday,Tuesday")]
    [InlineData(false, DayOfWeek.Wednesday, "Monday,Tuesday")]
    [InlineData(true, DayOfWeek.Tuesday, "Monday, Tuesday")]
    [InlineData(true, DayOfWeek.Monday, "monday,TUESDAY")]
    public void Convert_WithCommaSeparatedString(
        bool expected,
        DayOfWeek input,
        string parameter)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: parameter,
                culture: null));

    [Fact]
    public void Convert_WithEnumArray_MatchReturnsTrue()
        => Assert.True(
            (bool)converter.Convert(
                value: DayOfWeek.Monday,
                targetType: null,
                parameter: new[] { DayOfWeek.Monday, DayOfWeek.Tuesday },
                culture: null));

    [Fact]
    public void Convert_WithEnumArray_NoMatchReturnsFalse()
        => Assert.False(
            (bool)converter.Convert(
                value: DayOfWeek.Wednesday,
                targetType: null,
                parameter: new[] { DayOfWeek.Monday, DayOfWeek.Tuesday },
                culture: null));

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
        => Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(
                true,
                targetType: null,
                parameter: null,
                culture: null));
}