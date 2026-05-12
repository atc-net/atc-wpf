// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class EnumToInverseBoolValueConverterTests
{
    private readonly IValueConverter converter = new EnumToInverseBoolValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(EnumToInverseBoolValueConverter.Instance);

    [Theory]
    [InlineData(false, DayOfWeek.Monday, "Monday")]
    [InlineData(false, DayOfWeek.Friday, "Friday")]
    [InlineData(false, DayOfWeek.Monday, "monday")]
    [InlineData(true, DayOfWeek.Monday, "Tuesday")]
    [InlineData(true, DayOfWeek.Monday, "")]
    [InlineData(true, DayOfWeek.Monday, null)]
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
    public void Convert_WithNullValue_ReturnsTrue()
        => Assert.True(
            (bool)converter.Convert(
                value: null,
                targetType: null,
                parameter: "Monday",
                culture: null));

    [Fact]
    public void Convert_WithNonEnumValue_ReturnsTrue()
        => Assert.True(
            (bool)converter.Convert(
                value: "NotAnEnum",
                targetType: null,
                parameter: "Monday",
                culture: null));

    [Theory]
    [InlineData(false, DayOfWeek.Monday, "Monday,Tuesday")]
    [InlineData(false, DayOfWeek.Tuesday, "Monday,Tuesday")]
    [InlineData(true, DayOfWeek.Wednesday, "Monday,Tuesday")]
    [InlineData(false, DayOfWeek.Monday, "monday,TUESDAY")]
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
    public void Convert_WithEnumArray_NoMatchReturnsTrue()
        => Assert.True(
            (bool)converter.Convert(
                value: DayOfWeek.Wednesday,
                targetType: null,
                parameter: new[] { DayOfWeek.Monday, DayOfWeek.Tuesday },
                culture: null));

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
        => Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(
                false,
                targetType: null,
                parameter: null,
                culture: null));
}