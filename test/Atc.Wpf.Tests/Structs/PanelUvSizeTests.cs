namespace Atc.Wpf.Tests.Structs;

public sealed class PanelUvSizeTests
{
    [Fact]
    public void Equals_SameOrientationAndExtents_IsTrue()
    {
        var left = new PanelUvSize(Orientation.Horizontal, 10, 20);
        var right = new PanelUvSize(Orientation.Horizontal, 10, 20);

        Assert.True(left.Equals(right));
        Assert.True(left == right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentOrientationWithSameUv_IsFalse()
    {
        // Both have U = 10 and V = 20, but they describe transposed screen sizes.
        var horizontal = new PanelUvSize(Orientation.Horizontal, 10, 20);
        var vertical = new PanelUvSize(Orientation.Vertical, 20, 10);
        Assert.Equal(horizontal.U, vertical.U);
        Assert.Equal(horizontal.V, vertical.V);

        Assert.False(horizontal.Equals(vertical));
        Assert.True(horizontal != vertical);
        Assert.NotEqual(horizontal.ScreenSize, new Size(vertical.Width, vertical.Height));
    }

    [Fact]
    public void Equals_Object_UsesTheSameRule()
    {
        var horizontal = new PanelUvSize(Orientation.Horizontal, 10, 20);
        object vertical = new PanelUvSize(Orientation.Vertical, 20, 10);
        object same = new PanelUvSize(Orientation.Horizontal, 10, 20);

        Assert.False(horizontal.Equals(vertical));
        Assert.True(horizontal.Equals(same));
        Assert.False(horizontal.Equals(null));
    }
}