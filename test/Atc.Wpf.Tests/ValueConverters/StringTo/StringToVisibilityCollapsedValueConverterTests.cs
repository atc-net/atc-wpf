// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class StringToVisibilityCollapsedValueConverterTests
{
    private readonly IValueConverter converter = new StringToVisibilityCollapsedValueConverter();

    [Theory]
    [InlineData(Visibility.Collapsed, "Hidden", "Hidden")]
    [InlineData(Visibility.Collapsed, "Hidden", "hidden")]
    [InlineData(Visibility.Collapsed, "HIDDEN", "hidden")]
    [InlineData(Visibility.Visible, "Hidden", "Visible")]
    [InlineData(Visibility.Visible, "Hidden", "")]
    [InlineData(Visibility.Visible, "", "Hidden")]
    [InlineData(Visibility.Collapsed, "", "")]
    [InlineData(Visibility.Collapsed, null, null)]
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
    public void Convert_WithNullValueAndNonNullParameter_ReturnsVisible()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: "Hidden",
                culture: null));

    [Fact]
    public void Convert_WithNonNullValueAndNullParameter_ReturnsVisible()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: "Hidden",
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