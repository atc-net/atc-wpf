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
        var hsv = HsvColorUtil.ConvertRgbToHsv(r: red, g: green, b: blue);

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
                    var hsv = HsvColorUtil.ConvertRgbToHsv(r: red, g: green, b: blue);
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