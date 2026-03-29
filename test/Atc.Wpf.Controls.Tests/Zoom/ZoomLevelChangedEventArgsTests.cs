namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ZoomLevelChangedEventArgsTests
{
    [Fact]
    public void Constructor_SetsZoomValues()
    {
        var args = new ZoomLevelChangedEventArgs(1.0, 2.0, 0.1, 10.0);

        Assert.Equal(1.0, args.OldZoom);
        Assert.Equal(2.0, args.NewZoom);
    }

    [Fact]
    public void LevelOfDetail_AtMinimum_IsZero()
    {
        var args = new ZoomLevelChangedEventArgs(1.0, 0.1, 0.1, 10.0);

        Assert.Equal(0.0, args.LevelOfDetail, precision: 5);
    }

    [Fact]
    public void LevelOfDetail_AtMaximum_IsOne()
    {
        var args = new ZoomLevelChangedEventArgs(1.0, 10.0, 0.1, 10.0);

        Assert.Equal(1.0, args.LevelOfDetail, precision: 5);
    }

    [Fact]
    public void LevelOfDetail_AtMidpoint_IsNormalized()
    {
        var args = new ZoomLevelChangedEventArgs(1.0, 5.05, 0.1, 10.0);

        Assert.Equal(0.5, args.LevelOfDetail, precision: 1);
    }

    [Fact]
    public void LevelOfDetail_EqualMinMax_ReturnsOne()
    {
        var args = new ZoomLevelChangedEventArgs(1.0, 1.0, 1.0, 1.0);

        Assert.Equal(1.0, args.LevelOfDetail);
    }
}