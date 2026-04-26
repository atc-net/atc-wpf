namespace Atc.Wpf.Tests.Extensions;

public sealed class BrushExtensionsTests
{
    [Fact]
    public void IsOpaqueSolidColorBrush_returns_true_for_fully_opaque_solid_color()
    {
        var brush = new SolidColorBrush(Color.FromArgb(0xFF, 1, 2, 3));

        Assert.True(brush.IsOpaqueSolidColorBrush());
    }

    [Fact]
    public void IsOpaqueSolidColorBrush_returns_false_for_translucent_solid_color()
    {
        var brush = new SolidColorBrush(Color.FromArgb(0xFE, 1, 2, 3));

        Assert.False(brush.IsOpaqueSolidColorBrush());
    }

    [Fact]
    public void IsOpaqueSolidColorBrush_returns_false_for_non_solid_brush()
    {
        var brush = new LinearGradientBrush(Colors.Red, Colors.Blue, angle: 0);

        Assert.False(brush.IsOpaqueSolidColorBrush());
    }

    [Fact]
    public void IsEqualTo_returns_true_for_same_reference()
    {
        var brush = new SolidColorBrush(Colors.Red);

        Assert.True(brush.IsEqualTo(brush));
    }

    [Fact]
    public void IsEqualTo_returns_true_for_two_solid_brushes_with_same_color_and_opacity()
    {
        var first = new SolidColorBrush(Colors.Red) { Opacity = 0.5 };
        var second = new SolidColorBrush(Colors.Red) { Opacity = 0.5 };

        Assert.True(first.IsEqualTo(second));
    }

    [Fact]
    public void IsEqualTo_returns_false_for_solid_brushes_with_different_color()
    {
        var first = new SolidColorBrush(Colors.Red);
        var second = new SolidColorBrush(Colors.Blue);

        Assert.False(first.IsEqualTo(second));
    }

    [Fact]
    public void IsEqualTo_returns_false_for_solid_brushes_with_different_opacity()
    {
        var first = new SolidColorBrush(Colors.Red) { Opacity = 0.25 };
        var second = new SolidColorBrush(Colors.Red) { Opacity = 0.75 };

        Assert.False(first.IsEqualTo(second));
    }

    [Fact]
    public void IsEqualTo_returns_false_for_brushes_of_different_types()
    {
        var solid = new SolidColorBrush(Colors.Red);
        var linear = new LinearGradientBrush(Colors.Red, Colors.Red, angle: 0);

        Assert.False(solid.IsEqualTo(linear));
    }

    [Fact]
    public void IsEqualTo_returns_true_for_two_linear_gradients_with_matching_stops()
    {
        var first = new LinearGradientBrush(Colors.Red, Colors.Blue, angle: 45);
        var second = new LinearGradientBrush(Colors.Red, Colors.Blue, angle: 45);

        Assert.True(first.IsEqualTo(second));
    }

    [Fact]
    public void IsEqualTo_returns_false_for_two_linear_gradients_with_different_stop_count()
    {
        var first = new LinearGradientBrush(Colors.Red, Colors.Blue, angle: 0);
        var second = new LinearGradientBrush(
            new GradientStopCollection
            {
                new GradientStop(Colors.Red, 0d),
                new GradientStop(Colors.Yellow, 0.5),
                new GradientStop(Colors.Blue, 1d),
            });

        Assert.False(first.IsEqualTo(second));
    }

    [Fact]
    public void IsEqualTo_returns_false_for_two_radial_gradients_with_different_radius()
    {
        var first = new RadialGradientBrush(Colors.Red, Colors.Blue) { RadiusX = 0.5 };
        var second = new RadialGradientBrush(Colors.Red, Colors.Blue) { RadiusX = 0.75 };

        Assert.False(first.IsEqualTo(second));
    }

    [Fact]
    public void IsEqualTo_throws_for_null_brush()
    {
        var brush = new SolidColorBrush(Colors.Red);

        Assert.Throws<ArgumentNullException>(() =>
            brush.IsEqualTo(otherBrush: null!));
    }
}