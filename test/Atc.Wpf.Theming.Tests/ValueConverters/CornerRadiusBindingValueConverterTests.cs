namespace Atc.Wpf.Theming.Tests.ValueConverters;

public sealed class CornerRadiusBindingValueConverterTests
{
    private static readonly CornerRadius InputRadius = new(1, 2, 3, 4);

    public static TheoryData<RadiusType, CornerRadius> ParameterCases => new()
    {
        { RadiusType.None, InputRadius },
        { RadiusType.Left, new CornerRadius(0, 2, 3, 0) },
        { RadiusType.Top, new CornerRadius(0, 0, 3, 4) },
        { RadiusType.Right, new CornerRadius(1, 0, 0, 4) },
        { RadiusType.Bottom, new CornerRadius(1, 2, 0, 0) },
        { RadiusType.TopLeft, new CornerRadius(0, 2, 3, 4) },
        { RadiusType.TopRight, new CornerRadius(1, 0, 3, 4) },
        { RadiusType.BottomRight, new CornerRadius(1, 2, 0, 4) },
        { RadiusType.BottomLeft, new CornerRadius(1, 2, 3, 0) },
    };

    [Theory]
    [MemberData(nameof(ParameterCases))]
    public void Convert_zeros_corners_specified_by_parameter(
        RadiusType ignore,
        CornerRadius expected)
    {
        var actual = CornerRadiusBindingValueConverter.Instance.Convert(
            InputRadius,
            typeof(CornerRadius),
            ignore,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Convert_uses_IgnoreRadius_property_when_parameter_is_not_RadiusType()
    {
        var converter = new CornerRadiusBindingValueConverter
        {
            IgnoreRadius = RadiusType.Left,
        };

        var actual = converter.Convert(
            InputRadius,
            typeof(CornerRadius),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(new CornerRadius(0, 2, 3, 0), actual);
    }

    [Fact]
    public void Convert_returns_default_CornerRadius_when_value_is_not_CornerRadius()
    {
        var actual = CornerRadiusBindingValueConverter.Instance.Convert(
            "not a corner radius",
            typeof(CornerRadius),
            RadiusType.None,
            CultureInfo.InvariantCulture);

        Assert.Equal(default(CornerRadius), actual);
    }

    [Fact]
    public void ConvertBack_returns_DependencyProperty_UnsetValue()
    {
        var actual = CornerRadiusBindingValueConverter.Instance.ConvertBack(
            InputRadius,
            typeof(CornerRadius),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, actual);
    }
}