namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ViewBookmarkTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var state = new ViewportState(1.5, 10.0, 20.0);
        var bookmark = new ViewBookmark("Detail View", state);

        Assert.Equal("Detail View", bookmark.Name);
        Assert.Equal(1.5, bookmark.State.Zoom);
        Assert.Equal(10.0, bookmark.State.OffsetX);
        Assert.Equal(20.0, bookmark.State.OffsetY);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var state = new ViewportState(1.0, 0.0, 0.0);
        var a = new ViewBookmark("A", state);
        var b = new ViewBookmark("A", state);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equality_DifferentName_AreNotEqual()
    {
        var state = new ViewportState(1.0, 0.0, 0.0);
        var a = new ViewBookmark("A", state);
        var b = new ViewBookmark("B", state);

        Assert.NotEqual(a, b);
    }
}