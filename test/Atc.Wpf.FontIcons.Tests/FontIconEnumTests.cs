namespace Atc.Wpf.FontIcons.Tests;

public sealed class FontIconEnumTests
{
    public static TheoryData<Type> IconEnumTypes => new()
    {
        typeof(FontAwesomeSolidType),
        typeof(FontAwesomeRegularType),
        typeof(FontAwesomeBrandType),
        typeof(FontAwesomeSolid7Type),
        typeof(FontAwesomeRegular7Type),
        typeof(FontAwesomeBrand7Type),
        typeof(FontBootstrapType),
        typeof(FontMaterialDesignType),
        typeof(FontWeatherType),
        typeof(IcoFontType),
    };

    [Theory]
    [MemberData(nameof(IconEnumTypes))]
    public void IconEnum_should_define_None_member_with_value_zero(
        Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);

        var noneValue = Enum.Parse(enumType, "None");

        Assert.Equal(
            0,
            Convert.ToInt32(noneValue, CultureInfo.InvariantCulture));
    }

    [Theory]
    [MemberData(nameof(IconEnumTypes))]
    public void IconEnum_should_have_more_than_just_None(Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);

        var values = Enum.GetValues(enumType);

        Assert.True(
            values.Length > 1,
            $"{enumType.Name} should expose at least one icon besides None.");
    }
}