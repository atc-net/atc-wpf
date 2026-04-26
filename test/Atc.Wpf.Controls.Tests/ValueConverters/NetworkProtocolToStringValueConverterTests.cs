namespace Atc.Wpf.Controls.Tests.ValueConverters;

public sealed class NetworkProtocolToStringValueConverterTests
{
    [Theory]
    [InlineData(NetworkProtocolType.Ftp, "ftp")]
    [InlineData(NetworkProtocolType.Ftps, "ftps")]
    [InlineData(NetworkProtocolType.Http, "http")]
    [InlineData(NetworkProtocolType.Https, "https")]
    [InlineData(NetworkProtocolType.OpcTcp, "opc.tcp")]
    [InlineData(NetworkProtocolType.Tcp, "tcp")]
    [InlineData(NetworkProtocolType.Udp, "udp")]
    [InlineData(NetworkProtocolType.None, "")]
    public void Convert_maps_protocol_to_scheme(
        NetworkProtocolType protocol,
        string expected)
    {
        var actual = NetworkProtocolToStringValueConverter.Instance.Convert(
            protocol,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Convert_returns_empty_string_for_null_input()
    {
        var actual = NetworkProtocolToStringValueConverter.Instance.Convert(
            value: null,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void Convert_passes_string_through()
    {
        var actual = NetworkProtocolToStringValueConverter.Instance.Convert(
            "https",
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal("https", actual);
    }

    [Fact]
    public void Convert_returns_BindingDoNothing_for_unsupported_input()
    {
        var actual = NetworkProtocolToStringValueConverter.Instance.Convert(
            42,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(Binding.DoNothing, actual);
    }

    [Theory]
    [InlineData("ftp", NetworkProtocolType.Ftp)]
    [InlineData("ftps", NetworkProtocolType.Ftps)]
    [InlineData("http", NetworkProtocolType.Http)]
    [InlineData("https", NetworkProtocolType.Https)]
    [InlineData("opc.tcp", NetworkProtocolType.OpcTcp)]
    [InlineData("tcp", NetworkProtocolType.Tcp)]
    [InlineData("udp", NetworkProtocolType.Udp)]
    public void ConvertBack_maps_scheme_to_protocol(
        string scheme,
        NetworkProtocolType expected)
    {
        var actual = NetworkProtocolToStringValueConverter.Instance.ConvertBack(
            scheme,
            typeof(NetworkProtocolType),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ConvertBack_trims_whitespace_around_scheme()
    {
        var actual = NetworkProtocolToStringValueConverter.Instance.ConvertBack(
            "  https  ",
            typeof(NetworkProtocolType),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(NetworkProtocolType.Https, actual);
    }

    [Fact]
    public void ConvertBack_returns_None_for_null_input()
    {
        var actual = NetworkProtocolToStringValueConverter.Instance.ConvertBack(
            value: null,
            typeof(NetworkProtocolType),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(NetworkProtocolType.None, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ConvertBack_returns_None_for_blank_string(string blank)
    {
        var actual = NetworkProtocolToStringValueConverter.Instance.ConvertBack(
            blank,
            typeof(NetworkProtocolType),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(NetworkProtocolType.None, actual);
    }

    [Fact]
    public void ConvertBack_passes_NetworkProtocolType_through()
    {
        var actual = NetworkProtocolToStringValueConverter.Instance.ConvertBack(
            NetworkProtocolType.Tcp,
            typeof(NetworkProtocolType),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(NetworkProtocolType.Tcp, actual);
    }
}