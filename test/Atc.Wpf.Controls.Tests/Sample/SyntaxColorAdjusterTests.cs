namespace Atc.Wpf.Controls.Tests.Sample;

public sealed class SyntaxColorAdjusterTests
{
    [Fact]
    public void LightColors_AreLeftAlone()
    {
        var white = Colors.White;
        var lightYellow = Color.FromRgb(255, 240, 150);

        Assert.Equal(white, SyntaxColorAdjuster.EnsureReadableOnDark(white));
        Assert.Equal(lightYellow, SyntaxColorAdjuster.EnsureReadableOnDark(lightYellow));
    }

    [Fact]
    public void Black_BecomesLightGrey()
    {
        var adjusted = SyntaxColorAdjuster.EnsureReadableOnDark(Colors.Black);

        Assert.Equal(adjusted.R, adjusted.G);
        Assert.Equal(adjusted.G, adjusted.B);
        Assert.True(adjusted.R >= 200, $"expected a light grey, got {adjusted}");
    }

    [Theory]
    [InlineData(0, 0, 255)] // AvalonEdit keyword blue
    [InlineData(163, 21, 21)] // string red
    [InlineData(0, 128, 0)] // comment green
    public void DarkSaturatedColors_AreLiftedButKeepTheirHue(
        byte r,
        byte g,
        byte b)
    {
        var original = Color.FromRgb(r, g, b);

        var adjusted = SyntaxColorAdjuster.EnsureReadableOnDark(original);

        Assert.NotEqual(original, adjusted);
        Assert.True(Luminance(adjusted) > Luminance(original), "the colour must get lighter");
        Assert.True(Luminance(adjusted) > 0.4, $"expected a readable colour on a dark background, got {adjusted}");
        Assert.Equal(DominantChannel(original), DominantChannel(adjusted));
    }

    [Fact]
    public void Alpha_IsPreserved()
    {
        var translucent = Color.FromArgb(128, 0, 0, 128);

        var adjusted = SyntaxColorAdjuster.EnsureReadableOnDark(translucent);

        Assert.Equal(128, adjusted.A);
    }

    private static double Luminance(Color color)
        => ((0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B)) / 255d;

    private static char DominantChannel(Color color)
        => color.R >= color.G && color.R >= color.B
            ? 'R'
            : color.G >= color.B
                ? 'G'
                : 'B';
}