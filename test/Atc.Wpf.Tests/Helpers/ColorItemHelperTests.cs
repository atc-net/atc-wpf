// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Tests.Helpers;

public class ColorItemHelperTests
{
    [Theory]
    [InlineData(GlobalizationLcidConstants.Denmark, "Red", "#FFFF0000", "Rød")]
    [InlineData(GlobalizationLcidConstants.Germany, "Red", "#FFFF0000", "Rot")]
    [InlineData(GlobalizationLcidConstants.GreatBritain, "Red", "#FFFF0000", "Red")]
    [InlineData(GlobalizationLcidConstants.UnitedStates, "Red", "#FFFF0000", "Red")]
    [InlineData(GlobalizationLcidConstants.Denmark, "AntiqueWhite", "#FFFAEBD7", "Antik hvid")]
    [InlineData(GlobalizationLcidConstants.Germany, "AntiqueWhite", "#FFFAEBD7", "Antikweiß")]
    [InlineData(GlobalizationLcidConstants.GreatBritain, "AntiqueWhite", "#FFFAEBD7", "Antique White")]
    [InlineData(GlobalizationLcidConstants.UnitedStates, "AntiqueWhite", "#FFFAEBD7", "Antique White")]
    public void GetColorItems(int lcid, string findColorKey, string expectedHexCode, string expectedDisplayName)
    {
        var originalCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = new CultureInfo(lcid);

        var colorItems = ColorItemHelper.GetColorItems();
        var colorItem = colorItems.FirstOrDefault(x => x.Key == findColorKey);

        CultureInfo.CurrentUICulture = originalCulture;

        Assert.Equal(141, colorItems.Length);
        Assert.NotNull(colorItem);
        Assert.Equal(colorItem.DisplayHexCode, expectedHexCode);
        Assert.Equal(colorItem.DisplayName, expectedDisplayName);
    }

    [Theory]
    [InlineData(GlobalizationLcidConstants.Denmark, "Red", "#FFFF0000", "Rød")]
    [InlineData(GlobalizationLcidConstants.Germany, "Red", "#FFFF0000", "Rot")]
    [InlineData(GlobalizationLcidConstants.GreatBritain, "Red", "#FFFF0000", "Red")]
    [InlineData(GlobalizationLcidConstants.UnitedStates, "Red", "#FFFF0000", "Red")]
    public void GetBasicColorItems(int lcid, string findColorKey, string expectedHexCode, string expectedDisplayName)
    {
        var originalCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = new CultureInfo(lcid);

        var colorItems = ColorItemHelper.GetBasicColorItems();
        var colorItem = colorItems.FirstOrDefault(x => x.Key == findColorKey);

        CultureInfo.CurrentUICulture = originalCulture;

        Assert.Equal(16, colorItems.Length);
        Assert.NotNull(colorItem);
        Assert.Equal(colorItem.DisplayHexCode, expectedHexCode);
        Assert.Equal(colorItem.DisplayName, expectedDisplayName);
    }
}