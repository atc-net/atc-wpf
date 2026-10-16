namespace Atc.Wpf.Theming.Tests.Internal;

public sealed class PInvokeExtensionsTests
{
    [Fact]
    public void Offset_MovesTheRectangle()
    {
        var rect = new RECT
        {
            left = 10,
            top = 20,
            right = 110,
            bottom = 220,
        };

        rect.Offset(-5, 40);

        Assert.Equal(5, rect.left);
        Assert.Equal(60, rect.top);
        Assert.Equal(105, rect.right);
        Assert.Equal(260, rect.bottom);
    }

    [Fact]
    public void Offset_KeepsTheSize()
    {
        var rect = new RECT
        {
            left = 10,
            top = 20,
            right = 110,
            bottom = 220,
        };

        rect.Offset(33, -7);

        Assert.Equal(100, rect.GetWidth());
        Assert.Equal(200, rect.GetHeight());
    }
}