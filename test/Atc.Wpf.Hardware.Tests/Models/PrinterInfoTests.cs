namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class PrinterInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var info = new PrinterInfo(
            name: "Brother HL-L2350DW",
            fullName: @"\\PRINTSERVER\Brother HL-L2350DW",
            isLocal: false,
            isShared: true,
            isDefault: true,
            queueStatus: "None");

        Assert.Equal("Brother HL-L2350DW", info.Name);
        Assert.Equal(@"\\PRINTSERVER\Brother HL-L2350DW", info.FullName);
        Assert.False(info.IsLocal);
        Assert.True(info.IsShared);
        Assert.True(info.IsDefault);
        Assert.Equal("None", info.QueueStatus);
        Assert.Equal(@"\\PRINTSERVER\Brother HL-L2350DW", info.DeviceId);
        Assert.Equal("Brother HL-L2350DW", info.FriendlyName);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Theory]
    [InlineData(false, "Microsoft Print to PDF", "Microsoft Print to PDF")]
    [InlineData(true, "Brother HL-L2350DW", "Brother HL-L2350DW ★")]
    public void ToString_AppendsStarForDefault(
        bool isDefault,
        string name,
        string expected)
    {
        var info = new PrinterInfo(
            name: name,
            fullName: name,
            isLocal: true,
            isShared: false,
            isDefault: isDefault,
            queueStatus: "None");

        Assert.Equal(expected, info.ToString());
    }

    [Fact]
    public void State_RaisesPropertyChanged()
    {
        var info = new PrinterInfo(
            name: "Printer",
            fullName: "Printer",
            isLocal: true,
            isShared: false,
            isDefault: false,
            queueStatus: "None");

        var raised = false;
        info.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(PrinterInfo.State))
            {
                raised = true;
            }
        };

        info.State = DeviceState.Disconnected;

        Assert.True(raised);
    }
}