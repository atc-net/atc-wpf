namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class DiskDriveInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var info = new DiskDriveInfo(
            deviceId: @"C:\",
            friendlyName: "Windows",
            driveType: System.IO.DriveType.Fixed,
            isReady: true,
            totalSize: 500_000_000_000L,
            availableFreeSpace: 250_000_000_000L);

        Assert.Equal(@"C:\", info.DeviceId);
        Assert.Equal("Windows", info.FriendlyName);
        Assert.Equal(System.IO.DriveType.Fixed, info.DriveType);
        Assert.True(info.IsReady);
        Assert.Equal(500_000_000_000L, info.TotalSize);
        Assert.Equal(250_000_000_000L, info.AvailableFreeSpace);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Theory]
    [InlineData(@"C:\", "Windows", System.IO.DriveType.Fixed, @"C:\ Windows (Fixed)")]
    [InlineData(@"D:\", "", System.IO.DriveType.CDRom, @"D:\ (CDRom)")]
    [InlineData(@"E:\", @"E:\", System.IO.DriveType.Removable, @"E:\ (Removable)")]
    public void ToString_FormatsByLabelAndType(
        string root,
        string label,
        System.IO.DriveType type,
        string expected)
    {
        var info = new DiskDriveInfo(
            deviceId: root,
            friendlyName: label,
            driveType: type,
            isReady: true,
            totalSize: null,
            availableFreeSpace: null);

        Assert.Equal(expected, info.ToString());
    }

    [Fact]
    public void State_RaisesPropertyChanged()
    {
        var info = new DiskDriveInfo(
            deviceId: @"C:\",
            friendlyName: "OS",
            driveType: System.IO.DriveType.Fixed,
            isReady: true,
            totalSize: null,
            availableFreeSpace: null);

        var raised = false;
        info.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DiskDriveInfo.State))
            {
                raised = true;
            }
        };

        info.State = DeviceState.Disconnected;

        Assert.True(raised);
    }
}