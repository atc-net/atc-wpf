namespace Atc.Wpf.Tests.Media;

public sealed class HsvColorUtilTests
{
    [Theory]
    [InlineData(0, 15, 90)]
    [InlineData(255, 255, 255)]
    [InlineData(0, 0, 0)]
    [InlineData(89, 89, 89)]
    [InlineData(1, 2, 3)]
    [InlineData(254, 253, 252)]
    public void ConvertHsvToRgb_RoundTrips_ExactRgb(
        int red,
        int green,
        int blue)
    {
        var hsv = HsvColorUtil.ConvertRgbToHsv(Color.FromRgb((byte)red, (byte)green, (byte)blue));

        var rgb = HsvColorUtil.ConvertHsvToRgb(hsv.H, hsv.S, hsv.V);

        Assert.Equal(Color.FromRgb((byte)red, (byte)green, (byte)blue), rgb);
    }

    [Fact]
    public void ConvertHsvToRgb_RoundTrips_AcrossTheColorGrid()
    {
        var mismatches = new List<string>();

        for (var red = 0; red <= 255; red += 5)
        {
            for (var green = 0; green <= 255; green += 5)
            {
                for (var blue = 0; blue <= 255; blue += 5)
                {
                    var hsv = HsvColorUtil.ConvertRgbToHsv(Color.FromRgb((byte)red, (byte)green, (byte)blue));
                    var rgb = HsvColorUtil.ConvertHsvToRgb(hsv.H, hsv.S, hsv.V);
                    if (rgb.R != red || rgb.G != green || rgb.B != blue)
                    {
                        mismatches.Add($"({red},{green},{blue}) -> ({rgb.R},{rgb.G},{rgb.B})");
                    }
                }
            }
        }

        Assert.True(mismatches.Count == 0, $"{mismatches.Count} colours did not round-trip, e.g. {string.Join("; ", mismatches.Take(5))}");
    }

    [Theory]
    [InlineData(255, 0, 0, 0)]
    [InlineData(0, 255, 0, 120)]
    [InlineData(0, 0, 255, 240)]
    [InlineData(255, 255, 0, 60)]
    [InlineData(0, 255, 255, 180)]
    [InlineData(255, 0, 255, 300)]
    public void ConvertRgbToHsv_Color_UsesTheChannelsInRgbOrder(
        byte red,
        byte green,
        byte blue,
        double expectedHue)
    {
        var hsv = HsvColorUtil.ConvertRgbToHsv(Color.FromRgb(red, green, blue));

        Assert.Equal(expectedHue, hsv.H, 9);
        Assert.Equal(1d, hsv.S, 9);
        Assert.Equal(1d, hsv.V, 9);
    }

    [Fact]
    public void ConvertRgbToHsv_Color_PureGreen_HasHue120()
    {
        var hsv = HsvColorUtil.ConvertRgbToHsv(Colors.Lime);

        Assert.Equal(120d, hsv.H, 9);
    }

    [Fact]
    public void ConvertRgbToHsv_Color_PureBlue_HasHue240()
    {
        var hsv = HsvColorUtil.ConvertRgbToHsv(Colors.Blue);

        Assert.Equal(240d, hsv.H, 9);
    }

    [Theory]
    [InlineData(0, 255, 0)]
    [InlineData(0, 0, 255)]
    [InlineData(12, 200, 99)]
    [InlineData(99, 12, 200)]
    public void ConvertRgbToHsv_Color_RoundTripsThroughConvertHsvToRgb(
        byte red,
        byte green,
        byte blue)
    {
        var color = Color.FromRgb(red, green, blue);

        var hsv = HsvColorUtil.ConvertRgbToHsv(color);
        var rgb = HsvColorUtil.ConvertHsvToRgb(hsv.H, hsv.S, hsv.V);

        Assert.Equal(color, rgb);
    }

    [Fact]
    public void ConvertRgbToHsv_Color_IgnoresAlpha()
    {
        var opaque = HsvColorUtil.ConvertRgbToHsv(Color.FromArgb(255, 10, 20, 30));
        var translucent = HsvColorUtil.ConvertRgbToHsv(Color.FromArgb(10, 10, 20, 30));

        Assert.Equal((opaque.H, opaque.S, opaque.V), (translucent.H, translucent.S, translucent.V));
    }

    [Fact]
    public void ConvertHsvToRgb_FullValue_IsOpaqueWhite()
    {
        var rgb = HsvColorUtil.ConvertHsvToRgb(0, 0, 1);

        Assert.Equal(Colors.White, rgb);
    }

    [Fact]
    public void ConvertHsvToRgb_Hue360_IsTreatedAsHue0()
    {
        Assert.Equal(
            HsvColorUtil.ConvertHsvToRgb(0, 1, 1),
            HsvColorUtil.ConvertHsvToRgb(360, 1, 1));
    }
}