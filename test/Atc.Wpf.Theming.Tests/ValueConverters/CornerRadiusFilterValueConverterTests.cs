namespace Atc.Wpf.Theming.Tests.ValueConverters;

public sealed class CornerRadiusFilterValueConverterTests
{
    private static readonly CornerRadius InputRadius = new(1, 2, 3, 4);

    public static TheoryData<RadiusType, object> SideFilterCases => new()
    {
        { RadiusType.None, InputRadius },
        { RadiusType.Left, new CornerRadius(1, 0, 0, 4) },
        { RadiusType.Top, new CornerRadius(1, 2, 0, 0) },
        { RadiusType.Right, new CornerRadius(0, 2, 3, 0) },
        { RadiusType.Bottom, new CornerRadius(0, 0, 3, 4) },
    };

    public static TheoryData<RadiusType, double> CornerFilterCases => new()
    {
        { RadiusType.TopLeft, 1d },
        { RadiusType.TopRight, 2d },
        { RadiusType.BottomRight, 3d },
        { RadiusType.BottomLeft, 4d },
    };

    [Theory]
    [MemberData(nameof(SideFilterCases))]
    public void Convert_keeps_only_corners_for_a_side(
        RadiusType filter,
        object expected)
    {
        var actual = CornerRadiusFilterValueConverter.Instance.Convert(
            InputRadius,
            typeof(CornerRadius),
            filter,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(CornerFilterCases))]
    public void Convert_returns_double_for_single_corner_filter(
        RadiusType filter,
        double expected)
    {
        var actual = CornerRadiusFilterValueConverter.Instance.Convert(
            InputRadius,
            typeof(double),
            filter,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, Assert.IsType<double>(actual));
    }

    [Fact]
    public void Convert_uses_Filter_property_when_parameter_is_not_RadiusType()
    {
        var converter = new CornerRadiusFilterValueConverter
        {
            Filter = RadiusType.TopLeft,
        };

        var actual = converter.Convert(
            InputRadius,
            typeof(double),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(1d, Assert.IsType<double>(actual));
    }

    [Fact]
    public void Convert_returns_BindingDoNothing_when_value_is_not_CornerRadius()
    {
        var actual = CornerRadiusFilterValueConverter.Instance.Convert(
            "not a corner radius",
            typeof(CornerRadius),
            RadiusType.None,
            CultureInfo.InvariantCulture);

        Assert.Same(Binding.DoNothing, actual);
    }

    [Fact]
    public void ConvertBack_returns_DependencyProperty_UnsetValue()
    {
        var actual = CornerRadiusFilterValueConverter.Instance.ConvertBack(
            InputRadius,
            typeof(CornerRadius),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, actual);
    }
}