// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class StringToVisibilityVisibleValueConverterTests
{
    private readonly IValueConverter converter = new StringToVisibilityVisibleValueConverter();

    [Theory]
    [InlineData(Visibility.Visible, "Active", "Active")]
    [InlineData(Visibility.Visible, "Active", "active")]
    [InlineData(Visibility.Visible, "ACTIVE", "active")]
    [InlineData(Visibility.Collapsed, "Active", "Inactive")]
    [InlineData(Visibility.Collapsed, "Active", "")]
    [InlineData(Visibility.Collapsed, "", "Active")]
    [InlineData(Visibility.Visible, "", "")]
    [InlineData(Visibility.Visible, null, null)]
    public void Convert(
        Visibility expected,
        string? input,
        string? parameter)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: parameter,
                culture: null));

    [Fact]
    public void Convert_WithNullValueAndNonNullParameter_ReturnsCollapsed()
        => Assert.Equal(
            Visibility.Collapsed,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: "Active",
                culture: null));

    [Fact]
    public void Convert_WithNonNullValueAndNullParameter_ReturnsCollapsed()
        => Assert.Equal(
            Visibility.Collapsed,
            converter.Convert(
                value: "Active",
                targetType: null,
                parameter: null,
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