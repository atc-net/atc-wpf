namespace Atc.Wpf.Tests.Helpers;

public sealed class ColorHelperUnknownNameTests
{
    [Theory]
    [InlineData("NotAColor")]
    [InlineData("Rde")]
    [InlineData("Dark Red Ish")]
    public void GetColorFromString_returns_null_for_an_unknown_name(
        string input)
    {
        var actual = ColorHelper.GetColorFromString(input, GlobalizationConstants.EnglishCultureInfo);

        Assert.Null(actual);
    }

    [Fact]
    public void GetColorFromString_returns_null_for_an_unknown_name_in_another_culture()
    {
        var actual = ColorHelper.GetColorFromString("IkkeEnFarve", new CultureInfo(GlobalizationLcidConstants.Denmark));

        Assert.Null(actual);
    }

    [Fact]
    public void GetColorFromName_returns_null_for_an_unknown_name()
    {
        var actual = ColorHelper.GetColorFromName("NotAColor", GlobalizationConstants.EnglishCultureInfo);

        Assert.Null(actual);
    }

    [Fact]
    public void GetColorFromString_still_resolves_known_names_and_hex_values()
    {
        Assert.Equal(Colors.Red, ColorHelper.GetColorFromString("Red", GlobalizationConstants.EnglishCultureInfo));
        Assert.Equal(Colors.Red, ColorHelper.GetColorFromString("Rød", new CultureInfo(GlobalizationLcidConstants.Denmark)));
        Assert.Equal(Colors.Red, ColorHelper.GetColorFromString("#FFFF0000", GlobalizationConstants.EnglishCultureInfo));
    }

    [Fact]
    public void ColorNameToColorValueConverter_throws_for_an_unknown_name_instead_of_returning_transparent()
    {
        var exception = Record.Exception(() => ColorNameToColorValueConverter.Instance.Convert(
            "NotAColor",
            typeof(Color),
            parameter: null,
            CultureInfo.InvariantCulture));

        Assert.IsType<InvalidCastException>(exception);
    }
}