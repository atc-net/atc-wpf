namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ColorToSolidColorValueConverterTests
{
    [Fact]
    public void Convert_returns_DeepPink_brush_for_null_input()
    {
        var actual = ColorToSolidColorValueConverter.Instance.Convert(
            value: null,
            typeof(Color),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(Brushes.DeepPink, actual);
    }

    [Fact]
    public void Convert_throws_UnexpectedTypeException_for_non_color_input()
    {
        Assert.ThrowsAny<Exception>(() =>
            ColorToSolidColorValueConverter.Instance.Convert(
                "not a color",
                typeof(Color),
                parameter: null,
                CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Convert_overrides_alpha_to_255_while_preserving_RGB()
    {
        var translucent = Color.FromArgb(0x40, 0x12, 0x34, 0x56);

        var actual = ColorToSolidColorValueConverter.Instance.Convert(
            translucent,
            typeof(Color),
            parameter: null,
            CultureInfo.InvariantCulture);

        var color = Assert.IsType<Color>(actual);
        Assert.Equal(255, color.A);
        Assert.Equal(0x12, color.R);
        Assert.Equal(0x34, color.G);
        Assert.Equal(0x56, color.B);
    }

    [Fact]
    public void Convert_keeps_alpha_at_255_for_already_opaque_input()
    {
        var actual = ColorToSolidColorValueConverter.Instance.Convert(
            Colors.Red,
            typeof(Color),
            parameter: null,
            CultureInfo.InvariantCulture);

        var color = Assert.IsType<Color>(actual);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void ConvertBack_throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            ColorToSolidColorValueConverter.Instance.ConvertBack(
                Colors.Red,
                typeof(Color),
                parameter: null,
                CultureInfo.InvariantCulture));
    }
}