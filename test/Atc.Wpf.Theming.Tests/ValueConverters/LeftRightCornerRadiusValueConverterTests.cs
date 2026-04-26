namespace Atc.Wpf.Theming.Tests.ValueConverters;

public sealed class LeftRightCornerRadiusValueConverterTests
{
    [Fact]
    public void Convert_keeps_top_left_radius_on_the_right_side()
    {
        var input = new CornerRadius(5, 1, 2, 3);

        var actual = LeftRightCornerRadiusValueConverter.Instance.Convert(
            input,
            typeof(CornerRadius),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(new CornerRadius(0, 5, 5, 0), actual);
    }

    [Fact]
    public void Convert_returns_zero_radius_when_value_is_not_CornerRadius()
    {
        var actual = LeftRightCornerRadiusValueConverter.Instance.Convert(
            "not a corner radius",
            typeof(CornerRadius),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(new CornerRadius(0), actual);
    }

    [Fact]
    public void ConvertBack_returns_DependencyProperty_UnsetValue()
    {
        var actual = LeftRightCornerRadiusValueConverter.Instance.ConvertBack(
            value: null,
            typeof(CornerRadius),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, actual);
    }
}