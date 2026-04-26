namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ColorHexToColorValueConverterTests
{
    [Fact]
    public void Convert_color_to_hex_uses_AARRGGBB_format()
    {
        var color = Color.FromArgb(0xFF, 0x12, 0x34, 0x56);

        var actual = ColorHexToColorValueConverter.Instance.Convert(
            color,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal("#FF123456", actual);
    }

    [Theory]
    [InlineData("#FFFF0000", 0xFF, 0xFF, 0x00, 0x00)]
    [InlineData("FFFF0000", 0xFF, 0xFF, 0x00, 0x00)]
    [InlineData("#FF0000", 0xFF, 0xFF, 0x00, 0x00)]
    public void Convert_string_to_color_parses_hex(
        string hex,
        byte a,
        byte r,
        byte g,
        byte b)
    {
        var actual = ColorHexToColorValueConverter.Instance.Convert(
            hex,
            typeof(Color),
            parameter: null,
            CultureInfo.InvariantCulture);

        var color = Assert.IsType<Color>(actual);
        Assert.Equal(a, color.A);
        Assert.Equal(r, color.R);
        Assert.Equal(g, color.G);
        Assert.Equal(b, color.B);
    }

    [Fact]
    public void Convert_returns_BindingDoNothing_for_null_input()
    {
        var actual = ColorHexToColorValueConverter.Instance.Convert(
            value: null,
            typeof(Color),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(Binding.DoNothing, actual);
    }

    [Theory]
    [InlineData("#")]
    [InlineData("not-hex")]
    [InlineData("#FFFF")]
    public void Convert_returns_BindingDoNothing_for_invalid_length(
        string input)
    {
        var actual = ColorHexToColorValueConverter.Instance.Convert(
            input,
            typeof(Color),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(Binding.DoNothing, actual);
    }

    [Fact]
    public void Hex_round_trips_through_Convert_and_ConvertBack()
    {
        var original = Color.FromArgb(0xCC, 0xAB, 0xCD, 0xEF);

        var hex = ColorHexToColorValueConverter.Instance.Convert(
            original,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        var back = ColorHexToColorValueConverter.Instance.ConvertBack(
            hex,
            typeof(Color),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(original, Assert.IsType<Color>(back));
    }
}