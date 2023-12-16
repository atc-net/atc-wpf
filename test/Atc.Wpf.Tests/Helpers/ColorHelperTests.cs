// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Tests.Helpers;

public class ColorHelperTests
{
    [Fact]
    public void ColorCollectionCount()
    {
        var allColorNames = ColorHelper.GetAllColorNames(CultureInfo.CurrentUICulture);
        var baseColorNames = ColorHelper.GetBaseColorNames();
        var basicColorNames = ColorHelper.GetBasicColorNames();

        Assert.Equal(139, allColorNames.Count);
        Assert.Equal(141, baseColorNames.Count);
        Assert.Equal(16, basicColorNames.Count);
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
        var color = ColorHelper.GetColorFromString(input, new CultureInfo(lcid));

        // Assert
        Assert.NotNull(color);
        Assert.Equal(expectedColor, color);
    }

    [Theory]
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
        var color = ColorHelper.GetColorFromHex(hex);

        // Assert
        Assert.NotNull(color);
        Assert.Equal(expectedColor, color);
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
        var color = ColorHelper.GetColorFromName(name, new CultureInfo(lcid));

        // Assert
        Assert.NotNull(color);
        Assert.Equal(expectedColor, color);
    }
}