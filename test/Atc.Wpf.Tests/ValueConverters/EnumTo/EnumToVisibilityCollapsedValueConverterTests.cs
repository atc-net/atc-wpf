// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class EnumToVisibilityCollapsedValueConverterTests
{
    private readonly IValueConverter converter = new EnumToVisibilityCollapsedValueConverter();

    [Theory]
    [InlineData(Visibility.Collapsed, DayOfWeek.Monday, "Monday")]
    [InlineData(Visibility.Collapsed, DayOfWeek.Friday, "Friday")]
    [InlineData(Visibility.Collapsed, DayOfWeek.Monday, "monday")]
    [InlineData(Visibility.Visible, DayOfWeek.Monday, "Tuesday")]
    [InlineData(Visibility.Visible, DayOfWeek.Monday, "")]
    [InlineData(Visibility.Visible, DayOfWeek.Monday, null)]
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
    public void Convert_WithNullValue_ReturnsVisible()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: "Monday",
                culture: null));

    [Fact]
    public void Convert_WithNonEnumValue_ReturnsVisible()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: "NotAnEnum",
                targetType: null,
                parameter: "Monday",
                culture: null));

    [Fact]
    public void Convert_WithEnumParameter_ReturnsCollapsed()
        => Assert.Equal(
            Visibility.Collapsed,
            converter.Convert(
                value: DayOfWeek.Monday,
                targetType: null,
                parameter: DayOfWeek.Monday,
                culture: null));

    [Fact]
    public void Convert_WithDifferentEnumParameter_ReturnsVisible()
        => Assert.Equal(
            Visibility.Visible,
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