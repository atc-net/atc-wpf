namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class TopLevelWindowInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var hWnd = new IntPtr(0x12345);

        var info = new TopLevelWindowInfo(
            handle: hWnd,
            title: "Main",
            className: "Window",
            processId: 42,
            processName: "app");

        Assert.Equal(hWnd, info.Handle);
        Assert.Equal("Main", info.Title);
        Assert.Equal("Window", info.ClassName);
        Assert.Equal(42, info.ProcessId);
        Assert.Equal("app", info.ProcessName);
        Assert.Equal(hWnd.ToString(System.Globalization.CultureInfo.InvariantCulture), info.DeviceId);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Theory]
    [InlineData("Main", "Window", "Main")]
    [InlineData("", "Window", "(Window)")]
    public void FriendlyName_FallsBackToClassNameWhenTitleEmpty(
        string title,
        string className,
        string expected)
    {
        var info = new TopLevelWindowInfo(
            handle: IntPtr.Zero,
            title: title,
            className: className,
            processId: 1,
            processName: "app");

        Assert.Equal(expected, info.FriendlyName);
    }

    [Fact]
    public void ToString_IncludesProcessName()
    {
        var info = new TopLevelWindowInfo(
            handle: new IntPtr(1),
            title: "Title",
            className: "C",
            processId: 1,
            processName: "demo");

        Assert.Contains("demo", info.ToString(), StringComparison.Ordinal);
    }
}