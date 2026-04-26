namespace Atc.Wpf.Tests.Extensions;

public sealed class RectExtensionsTests
{
    [Fact]
    public void Deflate_shrinks_the_rectangle_by_the_thickness()
    {
        var rect = new Rect(0, 0, 100, 80);
        var thickness = new Thickness(5, 10, 5, 10);

        var actual = rect.Deflate(thickness);

        Assert.Equal(new Rect(5, 10, 90, 60), actual);
    }

    [Fact]
    public void Deflate_clamps_dimensions_to_zero_when_thickness_exceeds_size()
    {
        var rect = new Rect(0, 0, 10, 10);
        var thickness = new Thickness(20);

        var actual = rect.Deflate(thickness);

        Assert.Equal(0, actual.Width);
        Assert.Equal(0, actual.Height);
    }

    [Fact]
    public void Inflate_expands_the_rectangle_by_the_thickness()
    {
        var rect = new Rect(10, 20, 30, 40);
        var thickness = new Thickness(5);

        var actual = rect.Inflate(thickness);

        Assert.Equal(new Rect(5, 15, 40, 50), actual);
    }
}