namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class DisplayInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var info = new DisplayInfo(
            handle: new IntPtr(0x1234),
            deviceName: @"\\.\DISPLAY1",
            bounds: new System.Windows.Rect(0, 0, 1920, 1080),
            workingArea: new System.Windows.Rect(0, 0, 1920, 1040),
            isPrimary: true);

        Assert.Equal(new IntPtr(0x1234), info.Handle);
        Assert.Equal(@"\\.\DISPLAY1", info.DeviceName);
        Assert.Equal(@"\\.\DISPLAY1", info.DeviceId);
        Assert.Equal(@"\\.\DISPLAY1", info.FriendlyName);
        Assert.Equal(1920, info.Bounds.Width);
        Assert.Equal(1080, info.Bounds.Height);
        Assert.Equal(1040, info.WorkingArea.Height);
        Assert.True(info.IsPrimary);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Fact]
    public void ToString_PrimaryIncludesStarAndResolution()
    {
        var info = new DisplayInfo(
            handle: IntPtr.Zero,
            deviceName: @"\\.\DISPLAY1",
            bounds: new System.Windows.Rect(0, 0, 2560, 1440),
            workingArea: new System.Windows.Rect(0, 0, 2560, 1400),
            isPrimary: true);

        var rendered = info.ToString();

        Assert.Contains("★", rendered, StringComparison.Ordinal);
        Assert.Contains("2560×1440", rendered, StringComparison.Ordinal);
    }

    [Fact]
    public void ToString_NonPrimaryHasNoStar()
    {
        var info = new DisplayInfo(
            handle: IntPtr.Zero,
            deviceName: @"\\.\DISPLAY2",
            bounds: new System.Windows.Rect(1920, 0, 1920, 1080),
            workingArea: new System.Windows.Rect(1920, 0, 1920, 1040),
            isPrimary: false);

        var rendered = info.ToString();

        Assert.DoesNotContain("★", rendered, StringComparison.Ordinal);
        Assert.Contains("1920×1080", rendered, StringComparison.Ordinal);
    }

    [Fact]
    public void State_RaisesPropertyChanged()
    {
        var info = new DisplayInfo(
            handle: IntPtr.Zero,
            deviceName: @"\\.\DISPLAY1",
            bounds: System.Windows.Rect.Empty,
            workingArea: System.Windows.Rect.Empty,
            isPrimary: false);

        var raised = false;
        info.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DisplayInfo.State))
            {
                raised = true;
            }
        };

        info.State = DeviceState.Disconnected;

        Assert.True(raised);
    }
}