// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class EnumToVisibilityVisibleValueConverterTests
{
    private readonly IValueConverter converter = new EnumToVisibilityVisibleValueConverter();

    [Theory]
    [InlineData(Visibility.Visible, DayOfWeek.Monday, "Monday")]
    [InlineData(Visibility.Visible, DayOfWeek.Friday, "Friday")]
    [InlineData(Visibility.Visible, DayOfWeek.Monday, "monday")]
    [InlineData(Visibility.Collapsed, DayOfWeek.Monday, "Tuesday")]
    [InlineData(Visibility.Collapsed, DayOfWeek.Monday, "")]
    [InlineData(Visibility.Collapsed, DayOfWeek.Monday, null)]
    public void Convert(
        Visibility expected,
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
    public void Convert_WithNullValue_ReturnsCollapsed()
        => Assert.Equal(
            Visibility.Collapsed,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: "Monday",
                culture: null));

    [Fact]
    public void Convert_WithNonEnumValue_ReturnsCollapsed()
        => Assert.Equal(
            Visibility.Collapsed,
            converter.Convert(
                value: "NotAnEnum",
                targetType: null,
                parameter: "Monday",
                culture: null));

    [Fact]
    public void Convert_WithEnumParameter_ReturnsVisible()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: DayOfWeek.Monday,
                targetType: null,
                parameter: DayOfWeek.Monday,
                culture: null));

    [Fact]
    public void Convert_WithDifferentEnumParameter_ReturnsCollapsed()
        => Assert.Equal(
            Visibility.Collapsed,
            converter.Convert(
                value: DayOfWeek.Monday,
                targetType: null,
                parameter: DayOfWeek.Tuesday,
                culture: null));

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
        => Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(
                Visibility.Visible,
                targetType: null,
                parameter: null,
                culture: null));
}