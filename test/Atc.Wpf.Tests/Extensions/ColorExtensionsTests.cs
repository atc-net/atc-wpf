namespace Atc.Wpf.Tests.Extensions;

public sealed class ColorExtensionsTests
{
    [Fact]
    public void Lerp_returns_source_color_at_amount_zero()
    {
        var source = Color.FromRgb(0, 0, 0);
        var target = Color.FromRgb(255, 255, 255);

        var actual = source.Lerp(target, amount: 0f);

        Assert.Equal(0, actual.R);
        Assert.Equal(0, actual.G);
        Assert.Equal(0, actual.B);
    }

    [Fact]
    public void Lerp_returns_target_color_at_amount_one()
    {
        var source = Color.FromRgb(0, 0, 0);
        var target = Color.FromRgb(255, 255, 255);

        var actual = source.Lerp(target, amount: 1f);

        Assert.Equal(255, actual.R);
        Assert.Equal(255, actual.G);
        Assert.Equal(255, actual.B);
    }

    [Fact]
    public void Lerp_returns_midpoint_color_at_amount_half()
    {
        var source = Color.FromRgb(0, 0, 0);
        var target = Color.FromRgb(200, 100, 50);

        var actual = source.Lerp(target, amount: 0.5f);

        Assert.Equal(100, actual.R);
        Assert.Equal(50, actual.G);
        Assert.Equal(25, actual.B);
    }

    [Fact]
    public void GetHue_returns_zero_for_pure_red()
    {
        var hue = Colors.Red.GetHue();

        Assert.Equal(0d, hue, precision: 4);
    }

    [Fact]
    public void GetHue_returns_120_for_pure_green()
    {
        var hue = Colors.Lime.GetHue();

        Assert.Equal(120d, hue, precision: 4);
    }

    [Fact]
    public void GetHue_returns_240_for_pure_blue()
    {
        var hue = Colors.Blue.GetHue();

        Assert.Equal(240d, hue, precision: 4);
    }

    [Fact]
    public void GetBrightness_returns_zero_for_black()
    {
        Assert.Equal(0d, Colors.Black.GetBrightness(), precision: 4);
    }

    [Fact]
    public void GetBrightness_returns_one_for_white()
    {
        Assert.Equal(1d, Colors.White.GetBrightness(), precision: 4);
    }
}