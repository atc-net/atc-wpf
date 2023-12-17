// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Tests.Helpers;

public class SolidColorBrushHelperTests
{
    [Fact]
    public void BrushCollectionCount()
    {
        var allBrushNames = SolidColorBrushHelper.GetAllBrushNames(CultureInfo.CurrentUICulture);
        var baseBrushNames = SolidColorBrushHelper.GetBrushKeys();
        var basicBrushNames = SolidColorBrushHelper.GetBasicBrushNames();

        Assert.Equal(139, allBrushNames.Count);
        Assert.Equal(139, baseBrushNames.Count);
        Assert.Equal(16, basicBrushNames.Count);
    }

    [Theory]
    [InlineData("#FF00CED1", 0, 206, 209, GlobalizationLcidConstants.UnitedStates)]
    [InlineData("#FF2F4F4F", 47, 79, 79, GlobalizationLcidConstants.UnitedStates)]
    [InlineData("Red", 255, 0, 0, GlobalizationLcidConstants.UnitedStates)]
    [InlineData("Rød", 255, 0, 0, GlobalizationLcidConstants.Denmark)]
    public void GetBrushFromString(string input, byte r, byte g, byte b, int lcid)
    {
        // Arrange
        var expectedColor = Color.FromRgb(r, g, b);

        // Act
        var brush = SolidColorBrushHelper.GetBrushFromString(input, new CultureInfo(lcid));

        // Assert
        Assert.NotNull(brush);
        Assert.Equal(expectedColor, brush.Color);
    }

    [Theory]
    [InlineData("#FF333333", 0x33, 0x33, 0x33)]
    [InlineData("#FF666666", 0x66, 0x66, 0x66)]
    [InlineData("#FF00CED1", 0, 206, 209)]
    [InlineData("#00CED1", 0, 206, 209)]
    [InlineData("#FF2F4F4F", 47, 79, 79)]
    [InlineData("#FFFF0000", 255, 0, 0)]
    [InlineData("#FF0000", 255, 0, 0)]
    [InlineData("#F00", 255, 0, 0)]
    public void GetBrushFromHex(string hex, byte r, byte g, byte b)
    {
        // Arrange
        var expectedColor = Color.FromRgb(r, g, b);

        // Act
        var brush = SolidColorBrushHelper.GetBrushFromHex(hex);

        // Assert
        Assert.NotNull(brush);
        Assert.Equal(expectedColor, brush.Color);
    }

    [Theory]
    [InlineData("Red", 255, 0, 0, GlobalizationLcidConstants.UnitedStates)]
    [InlineData("Green", 0, 128, 0, GlobalizationLcidConstants.UnitedStates)]
    [InlineData("Blue", 0, 0, 255, GlobalizationLcidConstants.UnitedStates)]
    [InlineData("Rød", 255, 0, 0, GlobalizationLcidConstants.Denmark)]
    [InlineData("Grøn", 0, 128, 0, GlobalizationLcidConstants.Denmark)]
    [InlineData("Blå", 0, 0, 255, GlobalizationLcidConstants.Denmark)]
    public void GetBrushFromName(string name, byte r, byte g, byte b, int lcid)
    {
        // Arrange
        var expectedColor = Color.FromRgb(r, g, b);

        // Act
        var brush = SolidColorBrushHelper.GetBrushFromName(name, new CultureInfo(lcid));

        // Assert
        Assert.NotNull(brush);
        Assert.Equal(expectedColor, brush.Color);
    }

    [Theory]
    [InlineData("AliceBlue", "0xFFF0F8FF")]
    [InlineData("AntiqueWhite", "0xFFFAEBD7")]
    [InlineData("Aquamarine", "0xFF7FFFD4")]
    [InlineData("Azure", "0xFFF0FFFF")]
    [InlineData("Beige", "0xFFF5F5DC")]
    [InlineData("Bisque", "0xFFFFE4C4")]
    [InlineData("Black", "0xFF000000")]
    [InlineData("BlanchedAlmond", "0xFFFFEBCD")]
    [InlineData("Blue", "0xFF0000FF")]
    [InlineData("BlueViolet", "0xFF8A2BE2")]
    [InlineData("Brown", "0xFFA52A2A")]
    [InlineData("BurlyWood", "0xFFDEB887")]
    [InlineData("CadetBlue", "0xFF5F9EA0")]
    [InlineData("Chartreuse", "0xFF7FFF00")]
    [InlineData("Chocolate", "0xFFD2691E")]
    [InlineData("Coral", "0xFFFF7F50")]
    [InlineData("CornflowerBlue", "0xFF6495ED")]
    [InlineData("Cornsilk", "0xFFFFF8DC")]
    [InlineData("Crimson", "0xFFDC143C")]
    [InlineData("Cyan", "0xFF00FFFF")]
    [InlineData("DarkBlue", "0xFF00008B")]
    [InlineData("DarkCyan", "0xFF008B8B")]
    [InlineData("DarkGoldenrod", "0xFFB8860B")]
    [InlineData("DarkGray", "0xFFA9A9A9")]
    [InlineData("DarkGreen", "0xFF006400")]
    [InlineData("DarkKhaki", "0xFFBDB76B")]
    [InlineData("DarkMagenta", "0xFF8B008B")]
    [InlineData("DarkOliveGreen", "0xFF556B2F")]
    [InlineData("DarkOrange", "0xFFFF8C00")]
    [InlineData("DarkOrchid", "0xFF9932CC")]
    [InlineData("DarkRed", "0xFF8B0000")]
    [InlineData("DarkSalmon", "0xFFE9967A")]
    [InlineData("DarkSeaGreen", "0xFF8FBC8F")]
    [InlineData("DarkSlateBlue", "0xFF483D8B")]
    [InlineData("DarkSlateGray", "0xFF2F4F4F")]
    [InlineData("DarkTurquoise", "0xFF00CED1")]
    [InlineData("DarkViolet", "0xFF9400D3")]
    [InlineData("DeepPink", "0xFFFF1493")]
    [InlineData("DeepSkyBlue", "0xFF00BFFF")]
    [InlineData("DimGray", "0xFF696969")]
    [InlineData("DodgerBlue", "0xFF1E90FF")]
    [InlineData("Firebrick", "0xFFB22222")]
    [InlineData("FloralWhite", "0xFFFFFAF0")]
    [InlineData("ForestGreen", "0xFF228B22")]
    [InlineData("Gainsboro", "0xFFDCDCDC")]
    [InlineData("GhostWhite", "0xFFF8F8FF")]
    [InlineData("Gold", "0xFFFFD700")]
    [InlineData("Goldenrod", "0xFFDAA520")]
    [InlineData("Gray", "0xFF808080")]
    [InlineData("Green", "0xFF008000")]
    [InlineData("GreenYellow", "0xFFADFF2F")]
    [InlineData("Honeydew", "0xFFF0FFF0")]
    [InlineData("HotPink", "0xFFFF69B4")]
    [InlineData("IndianRed", "0xFFCD5C5C")]
    [InlineData("Indigo", "0xFF4B0082")]
    [InlineData("Ivory", "0xFFFFFFF0")]
    [InlineData("Khaki", "0xFFF0E68C")]
    [InlineData("Lavender", "0xFFE6E6FA")]
    [InlineData("LavenderBlush", "0xFFFFF0F5")]
    [InlineData("LawnGreen", "0xFF7CFC00")]
    [InlineData("LemonChiffon", "0xFFFFFACD")]
    [InlineData("LightBlue", "0xFFADD8E6")]
    [InlineData("LightCoral", "0xFFF08080")]
    [InlineData("LightCyan", "0xFFE0FFFF")]
    [InlineData("LightGoldenrodYellow", "0xFFFAFAD2")]
    [InlineData("LightGreen", "0xFF90EE90")]
    [InlineData("LightGray", "0xFFD3D3D3")]
    [InlineData("LightPink", "0xFFFFB6C1")]
    [InlineData("LightSalmon", "0xFFFFA07A")]
    [InlineData("LightSeaGreen", "0xFF20B2AA")]
    [InlineData("LightSkyBlue", "0xFF87CEFA")]
    [InlineData("LightSlateGray", "0xFF778899")]
    [InlineData("LightSteelBlue", "0xFFB0C4DE")]
    [InlineData("LightYellow", "0xFFFFFFE0")]
    [InlineData("Lime", "0xFF00FF00")]
    [InlineData("LimeGreen", "0xFF32CD32")]
    [InlineData("Linen", "0xFFFAF0E6")]
    [InlineData("Magenta", "0xFFFF00FF")]
    [InlineData("Maroon", "0xFF800000")]
    [InlineData("MediumAquamarine", "0xFF66CDAA")]
    [InlineData("MediumBlue", "0xFF0000CD")]
    [InlineData("MediumOrchid", "0xFFBA55D3")]
    [InlineData("MediumPurple", "0xFF9370DB")]
    [InlineData("MediumSeaGreen", "0xFF3CB371")]
    [InlineData("MediumSlateBlue", "0xFF7B68EE")]
    [InlineData("MediumSpringGreen", "0xFF00FA9A")]
    [InlineData("MediumTurquoise", "0xFF48D1CC")]
    [InlineData("MediumVioletRed", "0xFFC71585")]
    [InlineData("MidnightBlue", "0xFF191970")]
    [InlineData("MintCream", "0xFFF5FFFA")]
    [InlineData("MistyRose", "0xFFFFE4E1")]
    [InlineData("Moccasin", "0xFFFFE4B5")]
    [InlineData("NavajoWhite", "0xFFFFDEAD")]
    [InlineData("Navy", "0xFF000080")]
    [InlineData("OldLace", "0xFFFDF5E6")]
    [InlineData("Olive", "0xFF808000")]
    [InlineData("OliveDrab", "0xFF6B8E23")]
    [InlineData("Orange", "0xFFFFA500")]
    [InlineData("OrangeRed", "0xFFFF4500")]
    [InlineData("Orchid", "0xFFDA70D6")]
    [InlineData("PaleGoldenrod", "0xFFEEE8AA")]
    [InlineData("PaleGreen", "0xFF98FB98")]
    [InlineData("PaleTurquoise", "0xFFAFEEEE")]
    [InlineData("PaleVioletRed", "0xFFDB7093")]
    [InlineData("PapayaWhip", "0xFFFFEFD5")]
    [InlineData("PeachPuff", "0xFFFFDAB9")]
    [InlineData("Peru", "0xFFCD853F")]
    [InlineData("Pink", "0xFFFFC0CB")]
    [InlineData("Plum", "0xFFDDA0DD")]
    [InlineData("PowderBlue", "0xFFB0E0E6")]
    [InlineData("Purple", "0xFF800080")]
    [InlineData("Red", "0xFFFF0000")]
    [InlineData("RosyBrown", "0xFFBC8F8F")]
    [InlineData("RoyalBlue", "0xFF4169E1")]
    [InlineData("SaddleBrown", "0xFF8B4513")]
    [InlineData("Salmon", "0xFFFA8072")]
    [InlineData("SandyBrown", "0xFFF4A460")]
    [InlineData("SeaGreen", "0xFF2E8B57")]
    [InlineData("SeaShell", "0xFFFFF5EE")]
    [InlineData("Sienna", "0xFFA0522D")]
    [InlineData("Silver", "0xFFC0C0C0")]
    [InlineData("SkyBlue", "0xFF87CEEB")]
    [InlineData("SlateBlue", "0xFF6A5ACD")]
    [InlineData("SlateGray", "0xFF708090")]
    [InlineData("Snow", "0xFFFFFAFA")]
    [InlineData("SpringGreen", "0xFF00FF7F")]
    [InlineData("SteelBlue", "0xFF4682B4")]
    [InlineData("Tan", "0xFFD2B48C")]
    [InlineData("Teal", "0xFF008080")]
    [InlineData("Thistle", "0xFFD8BFD8")]
    [InlineData("Tomato", "0xFFFF6347")]
    [InlineData("Transparent", "0x00FFFFFF")]
    [InlineData("Turquoise", "0xFF40E0D0")]
    [InlineData("Violet", "0xFFEE82EE")]
    [InlineData("Wheat", "0xFFF5DEB3")]
    [InlineData("White", "0xFFFFFFFF")]
    [InlineData("WhiteSmoke", "0xFFF5F5F5")]
    [InlineData("Yellow", "0xFFFFFF00")]
    [InlineData("YellowGreen", "0xFF9ACD32")]
    public void GetBrushKeyFromHex(string expectedColorKey, string hexValue)
    {
        // Act
        var brushKey = SolidColorBrushHelper.GetBrushKeyFromHex(hexValue);

        // Assert
        Assert.NotNull(brushKey);
        Assert.Equal(expectedColorKey, brushKey);
    }
}