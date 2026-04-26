namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ThicknessFilterValueConverterTests
{
    private static readonly Thickness InputThickness = new(1, 2, 3, 4);

    public static TheoryData<LeftTopRightBottomType, Thickness> ParameterCases
        => new()
    {
        { LeftTopRightBottomType.None, InputThickness },
        { LeftTopRightBottomType.Left, new Thickness(1, 0, 0, 0) },
        { LeftTopRightBottomType.Top, new Thickness(0, 2, 0, 0) },
        { LeftTopRightBottomType.Right, new Thickness(0, 0, 3, 0) },
        { LeftTopRightBottomType.Bottom, new Thickness(0, 0, 0, 4) },
    };

    [Theory]
    [MemberData(nameof(ParameterCases))]
    public void Convert_keeps_only_the_side_specified_by_parameter(
        LeftTopRightBottomType filter,
        Thickness expected)
    {
        var actual = ThicknessFilterValueConverter.Instance.Convert(
            InputThickness,
            typeof(Thickness),
            filter,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Convert_uses_Filter_property_when_parameter_is_not_LeftTopRightBottomType()
    {
        var converter = new ThicknessFilterValueConverter
        {
            Filter = LeftTopRightBottomType.Top,
        };

        var actual = converter.Convert(
            InputThickness,
            typeof(Thickness),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(new Thickness(0, 2, 0, 0), actual);
    }

    [Fact]
    public void Convert_returns_BindingDoNothing_when_value_is_not_a_Thickness()
    {
        var actual = ThicknessFilterValueConverter.Instance.Convert(
            "not a thickness",
            typeof(Thickness),
            LeftTopRightBottomType.None,
            CultureInfo.InvariantCulture);

        Assert.Same(Binding.DoNothing, actual);
    }

    [Fact]
    public void ConvertBack_returns_DependencyProperty_UnsetValue()
    {
        var actual = ThicknessFilterValueConverter.Instance.ConvertBack(
            InputThickness,
            typeof(Thickness),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, actual);
    }
}