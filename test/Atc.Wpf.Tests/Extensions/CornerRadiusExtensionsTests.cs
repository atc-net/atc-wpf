namespace Atc.Wpf.Tests.Extensions;

public sealed class CornerRadiusExtensionsTests
{
    [Fact]
    public void IsValid_returns_true_for_non_negative_finite_radius()
    {
        var radius = new CornerRadius(0, 1, 2, 3);

        Assert.True(radius.IsValid(
            allowNegative: false,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void IsValid_returns_false_for_negative_corner_when_negatives_disallowed()
    {
        var radius = new CornerRadius(-1, 1, 2, 3);

        Assert.False(radius.IsValid(
            allowNegative: false,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void IsValid_returns_true_for_negative_corner_when_negatives_allowed()
    {
        var radius = new CornerRadius(-1, 1, 2, 3);

        Assert.True(radius.IsValid(
            allowNegative: true,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void IsValid_returns_false_for_NaN_when_NaN_disallowed()
    {
        var radius = new CornerRadius(double.NaN, 1, 2, 3);

        Assert.False(radius.IsValid(
            allowNegative: false,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void IsValid_returns_false_for_PositiveInfinity_when_disallowed()
    {
        var radius = new CornerRadius(double.PositiveInfinity, 1, 2, 3);

        Assert.False(radius.IsValid(
            allowNegative: false,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void IsValid_returns_false_for_NegativeInfinity_when_disallowed()
    {
        var radius = new CornerRadius(double.NegativeInfinity, 1, 2, 3);

        Assert.False(radius.IsValid(
            allowNegative: true,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false));
    }

    [Fact]
    public void IsZero_returns_true_for_default_radius()
    {
        Assert.True(default(CornerRadius).IsZero());
    }

    [Fact]
    public void IsZero_returns_false_when_any_corner_is_non_zero()
    {
        Assert.False(new CornerRadius(0, 0, 0, 0.5).IsZero());
    }

    [Fact]
    public void IsUniform_returns_true_when_all_corners_match()
    {
        Assert.True(new CornerRadius(5, 5, 5, 5).IsUniform());
    }

    [Fact]
    public void IsUniform_returns_false_when_any_corner_differs()
    {
        Assert.False(new CornerRadius(5, 5, 5, 6).IsUniform());
    }
}