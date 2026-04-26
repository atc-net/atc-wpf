namespace Atc.Wpf.Theming.Tests.ValueConverters;

public sealed class ColorToNameValueConverterTests
{
    [Fact]
    public void Convert_returns_name_for_a_well_known_color()
    {
        var actual = ColorToNameValueConverter.Instance.Convert(
            Colors.Red,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.IsType<string>(actual);
        Assert.False(string.IsNullOrWhiteSpace((string)actual));
    }

    [Fact]
    public void Convert_returns_pink_when_value_is_not_a_color()
    {
        var actual = ColorToNameValueConverter.Instance.Convert(
            "not a color",
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(Colors.Pink, actual);
    }

    [Fact]
    public void Convert_multi_finds_a_color_in_the_values_array()
    {
        var values = new object[] { "extra", Colors.Blue, 42 };

        var actual = ColorToNameValueConverter.Instance.Convert(
            values,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.IsType<string>(actual);
        Assert.False(string.IsNullOrWhiteSpace((string)actual));
    }

    [Fact]
    public void Convert_multi_returns_pink_when_no_color_present()
    {
        var values = new object[] { "extra", 42 };

        var actual = ColorToNameValueConverter.Instance.Convert(
            values,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(Colors.Pink, actual);
    }

    [Fact]
    public void ConvertBack_returns_BindingDoNothing_when_value_is_not_a_string()
    {
        var actual = ColorToNameValueConverter.Instance.ConvertBack(
            42,
            typeof(Color),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(Binding.DoNothing, actual);
    }

    [Fact]
    public void ConvertBack_multi_throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            ColorToNameValueConverter.Instance.ConvertBack(
                value: null,
                [typeof(Color)],
                parameter: null,
                CultureInfo.InvariantCulture));
    }
}