namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ZoomGridOverlayTests
{
    [StaTheory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void BaseGridSpacing_InvalidValue_Throws(double value)
    {
        // A non-positive or non-finite spacing makes the adaptive-spacing loop in OnRender never terminate.
        var overlay = new ZoomGridOverlay(new ZoomBox());

        Assert.Throws<ArgumentOutOfRangeException>(() => overlay.BaseGridSpacing = value);
        Assert.Equal(50.0, overlay.BaseGridSpacing);
    }

    [StaFact]
    public void BaseGridSpacing_PositiveValue_IsAccepted()
    {
        var overlay = new ZoomGridOverlay(new ZoomBox());

        overlay.BaseGridSpacing = 0.25;

        Assert.Equal(0.25, overlay.BaseGridSpacing);
    }
}