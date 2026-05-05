namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class UsbCameraFormatTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var format = new UsbCameraFormat(
            Width: 1920,
            Height: 1080,
            FrameRate: 30.0,
            Subtype: "NV12");

        Assert.Equal(1920u, format.Width);
        Assert.Equal(1080u, format.Height);
        Assert.Equal(30.0, format.FrameRate);
        Assert.Equal("NV12", format.Subtype);
    }

    [Theory]
    [InlineData(1920u, 1080u, 30.0, "NV12", "1920×1080 @ 30 fps (NV12)")]
    [InlineData(640u, 480u, 15.5, "MJPG", "640×480 @ 15.5 fps (MJPG)")]
    public void ToString_FormatsAsResolutionFpsSubtype(
        uint width,
        uint height,
        double fps,
        string subtype,
        string expected)
    {
        var format = new UsbCameraFormat(
            Width: width,
            Height: height,
            FrameRate: fps,
            Subtype: subtype);

        Assert.Equal(expected, format.ToString());
    }

    [Fact]
    public void Equality_IsValueBased()
    {
        var a = new UsbCameraFormat(1920, 1080, 30, "NV12");
        var b = new UsbCameraFormat(1920, 1080, 30, "NV12");
        var c = new UsbCameraFormat(1920, 1080, 60, "NV12");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}