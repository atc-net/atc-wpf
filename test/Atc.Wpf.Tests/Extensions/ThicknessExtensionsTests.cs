namespace Atc.Wpf.Tests.Extensions;

public sealed class ThicknessExtensionsTests
{
    [Fact]
    public void IsValid_returns_true_for_non_negative_finite_thickness()
    {
        Assert.True(new Thickness(0, 1, 2, 3).IsValid(
            allowNegative: false,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void IsValid_returns_false_for_negative_side_when_negatives_disallowed()
    {
        Assert.False(new Thickness(-1, 1, 2, 3).IsValid(
            allowNegative: false,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void IsValid_returns_false_for_NaN_when_disallowed()
    {
        Assert.False(new Thickness(double.NaN, 1, 2, 3).IsValid(
            allowNegative: false,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void IsValid_returns_false_for_PositiveInfinity_when_disallowed()
    {
        Assert.False(new Thickness(double.PositiveInfinity, 1, 2, 3).IsValid(
            allowNegative: false,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void IsValid_returns_false_for_NegativeInfinity_when_disallowed()
    {
        Assert.False(new Thickness(1, double.NegativeInfinity, 2, 3).IsValid(
            allowNegative: true,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void CollapseThickness_sums_left_plus_right_into_width_and_top_plus_bottom_into_height()
    {
        var actual = new Thickness(1, 2, 3, 4).CollapseThickness();

        Assert.Equal(new Size(4, 6), actual);
    }

    [Fact]
    public void IsZero_returns_true_for_default_thickness()
    {
        Assert.True(default(Thickness).IsZero());
    }

    [Fact]
    public void IsZero_returns_false_when_any_side_is_non_zero()
    {
        Assert.False(new Thickness(0, 0, 0.5, 0).IsZero());
    }

    [Fact]
    public void IsUniform_returns_true_when_all_sides_match()
    {
        Assert.True(new Thickness(5).IsUniform());
    }

    [Fact]
    public void IsUniform_returns_false_when_any_side_differs()
    {
        Assert.False(new Thickness(5, 5, 5, 6).IsUniform());
    }
}