// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Components.Tests.ValueConverters;

public sealed class BoolToTextWrappingValueConverterTests
{
    private readonly IValueConverter converter = new BoolToTextWrappingValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(BoolToTextWrappingValueConverter.Instance);

    [Theory]
    [InlineData(TextWrapping.Wrap, true)]
    [InlineData(TextWrapping.NoWrap, false)]
    public void Convert(
        TextWrapping expected,
        bool input)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_Null_ReturnsNoWrap()
        => Assert.Equal(
            TextWrapping.NoWrap,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Theory]
    [InlineData(true, TextWrapping.Wrap)]
    [InlineData(false, TextWrapping.NoWrap)]
    [InlineData(false, TextWrapping.WrapWithOverflow)]
    public void ConvertBack(
        bool expected,
        TextWrapping input)
        => Assert.Equal(
            expected,
            converter.ConvertBack(
                input,
                targetType: null,
                parameter: null,
                culture: CultureInfo.InvariantCulture));
}