namespace Atc.Wpf.Theming.Tests.Theming;

public sealed class HighContrastColorSchemeTests
{
    private static readonly HighContrastPalette Palette = new(
        Window: Color.FromRgb(0x00, 0x00, 0x00),
        WindowText: Color.FromRgb(0xFF, 0xFF, 0xFF),
        Highlight: Color.FromRgb(0x1A, 0xEB, 0xFF),
        HighlightText: Color.FromRgb(0x00, 0x00, 0x01),
        GrayText: Color.FromRgb(0x3F, 0xF2, 0x3F));

    private static readonly Dictionary<string, string> DarkBase = new(StringComparer.Ordinal)
    {
        ["AtcApps.Colors.ThemeBackground"] = "#FF1E1E1E",
        ["AtcApps.Colors.ThemeBackground3"] = "#FF3D3D3D",
        ["AtcApps.Colors.ThemeForeground"] = "#FFFFFFFF",
        ["AtcApps.Colors.ThemeForeground4"] = "#FF969696",
        ["AtcApps.Colors.Gray1"] = "#FFF9F9F9",
        ["AtcApps.Colors.Gray10"] = "#FF2F2F2F",
        ["AtcApps.Colors.Gray.SemiTransparent"] = "#40808080",
        ["AtcApps.Colors.SliderTrack.Disabled"] = "#FF383838",
        ["AtcApps.Brushes.SystemControlTransientBorder.Opacity"] = "0.36",
    };

    private static readonly Dictionary<string, string> LightBase = new(StringComparer.Ordinal)
    {
        ["AtcApps.Colors.ThemeBackground"] = "#FFFFFFFF",
        ["AtcApps.Colors.ThemeForeground"] = "#FF000000",
        ["AtcApps.Colors.Gray1"] = "#FF333333",
        ["AtcApps.Colors.Gray10"] = "#FFF7F7F7",
    };

    [Theory]
    [InlineData("AtcApps.Colors.AccentBase")]
    [InlineData("AtcApps.Colors.Accent")]
    [InlineData("AtcApps.Colors.Accent2")]
    [InlineData("AtcApps.Colors.Accent3")]
    [InlineData("AtcApps.Colors.Accent4")]
    [InlineData("AtcApps.Colors.Highlight")]
    public void Fill_AccentColorsUseTheHighlightColor(string key)
    {
        var values = Fill(DarkBase);

        Assert.Equal(Palette.Highlight, Parse(values[key]));
    }

    [Fact]
    public void Fill_TextOnTheAccentUsesTheHighlightTextColor()
    {
        var values = Fill(DarkBase);

        Assert.Equal(Palette.HighlightText, Parse(values["AtcApps.Colors.IdealForeground"]));
    }

    [Theory]
    [InlineData("AtcApps.Colors.ThemeBackground")]
    [InlineData("AtcApps.Colors.ThemeBackground3")]
    [InlineData("AtcApps.Colors.Gray10")]
    public void Fill_DarkBackgroundShadesUseTheWindowColor(string key)
    {
        var values = Fill(DarkBase);

        Assert.Equal(Palette.Window, Parse(values[key]));
    }

    [Theory]
    [InlineData("AtcApps.Colors.ThemeForeground")]
    [InlineData("AtcApps.Colors.ThemeForeground4")]
    [InlineData("AtcApps.Colors.Gray1")]
    public void Fill_DarkForegroundShadesUseTheWindowTextColor(string key)
    {
        var values = Fill(DarkBase);

        Assert.Equal(Palette.WindowText, Parse(values[key]));
    }

    [Fact]
    public void Fill_LightBase_ClassifiesShadesAgainstItsOwnBackground()
    {
        var values = Fill(LightBase);

        Assert.Equal(Palette.Window, Parse(values["AtcApps.Colors.Gray10"]));
        Assert.Equal(Palette.WindowText, Parse(values["AtcApps.Colors.Gray1"]));
    }

    [Fact]
    public void Fill_DisabledColorsUseTheGrayTextColor()
    {
        var values = Fill(DarkBase);

        Assert.Equal(Palette.GrayText, Parse(values["AtcApps.Colors.SliderTrack.Disabled"]));
    }

    [Fact]
    public void Fill_KeepsTheTransparencyOfAColor()
    {
        var values = Fill(DarkBase);

        var color = Parse(values["AtcApps.Colors.Gray.SemiTransparent"]);
        Assert.Equal(0x40, color.A);
    }

    [Fact]
    public void Fill_SkipsValuesThatAreNotColors()
    {
        var values = Fill(DarkBase);

        Assert.False(values.ContainsKey("AtcApps.Brushes.SystemControlTransientBorder.Opacity"));
    }

    [StaFact]
    public void MakeBordersVisible_ABorderInTheWindowColorGetsTheWindowTextColor()
    {
        var resources = new ResourceDictionary
        {
            ["AtcApps.Brushes.ListBox.Border"] = new SolidColorBrush(Palette.Window),
            ["AtcApps.Brushes.Separator"] = new SolidColorBrush(Palette.Window),
        };

        HighContrastColorScheme.MakeBordersVisible(resources, Palette);

        Assert.Equal(Palette.WindowText, ((SolidColorBrush)resources["AtcApps.Brushes.ListBox.Border"]).Color);
        Assert.Equal(Palette.WindowText, ((SolidColorBrush)resources["AtcApps.Brushes.Separator"]).Color);
    }

    [StaFact]
    public void MakeBordersVisible_KeepsAVisibleBorderAndOtherBrushes()
    {
        var resources = new ResourceDictionary
        {
            ["AtcApps.Brushes.Accent.Border"] = new SolidColorBrush(Palette.Highlight),
            ["AtcApps.Brushes.ThemeBackground"] = new SolidColorBrush(Palette.Window),
        };

        HighContrastColorScheme.MakeBordersVisible(resources, Palette);

        Assert.Equal(Palette.Highlight, ((SolidColorBrush)resources["AtcApps.Brushes.Accent.Border"]).Color);
        Assert.Equal(Palette.Window, ((SolidColorBrush)resources["AtcApps.Brushes.ThemeBackground"]).Color);
    }

    private static Dictionary<string, string> Fill(
        Dictionary<string, string> baseValues)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        HighContrastColorScheme.Fill(values, baseValues, Palette);
        return values;
    }

    private static Color Parse(string value)
        => (Color)ColorConverter.ConvertFromString(value);
}