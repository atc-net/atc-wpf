namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ViewportStateTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var state = new ViewportState(2.5, 100.0, 200.0);

        Assert.Equal(2.5, state.Zoom);
        Assert.Equal(100.0, state.OffsetX);
        Assert.Equal(200.0, state.OffsetY);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var a = new ViewportState(1.0, 50.0, 75.0);
        var b = new ViewportState(1.0, 50.0, 75.0);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        var a = new ViewportState(1.0, 50.0, 75.0);
        var b = new ViewportState(2.0, 50.0, 75.0);

        Assert.NotEqual(a, b);
    }
}