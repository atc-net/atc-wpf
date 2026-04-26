namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ThicknessBindingValueConverterTests
{
    private static readonly Thickness InputThickness = new(1, 2, 3, 4);

    public static TheoryData<LeftTopRightBottomType, Thickness> ParameterCases
        => new()
    {
        { LeftTopRightBottomType.None, InputThickness },
        { LeftTopRightBottomType.Left, new Thickness(0, 2, 3, 4) },
        { LeftTopRightBottomType.Top, new Thickness(1, 0, 3, 4) },
        { LeftTopRightBottomType.Right, new Thickness(1, 2, 0, 4) },
        { LeftTopRightBottomType.Bottom, new Thickness(1, 2, 3, 0) },
    };

    [Theory]
    [MemberData(nameof(ParameterCases))]
    public void Convert_zeros_the_side_specified_by_parameter(
        LeftTopRightBottomType ignore,
        Thickness expected)
    {
        var actual = ThicknessBindingValueConverter.Instance.Convert(
            InputThickness,
            typeof(Thickness),
            ignore,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Convert_uses_IgnoreThicknessSide_property_when_parameter_is_not_LeftTopRightBottomType()
    {
        var converter = new ThicknessBindingValueConverter
        {
            IgnoreThicknessSide = LeftTopRightBottomType.Right,
        };

        var actual = converter.Convert(
            InputThickness,
            typeof(Thickness),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(new Thickness(1, 2, 0, 4), actual);
    }

    [Fact]
    public void Convert_returns_default_Thickness_when_value_is_not_a_Thickness()
    {
        var actual = ThicknessBindingValueConverter.Instance.Convert(
            "not a thickness",
            typeof(Thickness),
            LeftTopRightBottomType.None,
            CultureInfo.InvariantCulture);

        Assert.Equal(default(Thickness), actual);
    }

    [Fact]
    public void ConvertBack_returns_DependencyProperty_UnsetValue()
    {
        var actual = ThicknessBindingValueConverter.Instance.ConvertBack(
            InputThickness,
            typeof(Thickness),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, actual);
    }
}