namespace Atc.Wpf.Hardware.Tests.Services.Internal;

public sealed class UsbIdParserTests
{
    [Theory]
    [InlineData(@"\\?\USB#VID_2341&PID_0043#abc", "2341", "0043")]
    [InlineData(@"\\?\USB#VID_1A86&PID_7523#5&abc#", "1A86", "7523")]
    [InlineData(@"\\?\USB#vid_FFff&pid_aaaa#x", "FFFF", "AAAA")]
    public void Parse_ExtractsVendorAndProduct(
        string deviceId,
        string expectedVid,
        string expectedPid)
    {
        var (vid, pid) = UsbIdParser.Parse(deviceId);

        Assert.Equal(expectedVid, vid);
        Assert.Equal(expectedPid, pid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("nothing-here")]
    [InlineData(@"\\?\HID#")]
    public void Parse_ReturnsNullsForUnrecognized(string deviceId)
    {
        var (vid, pid) = UsbIdParser.Parse(deviceId);

        Assert.Null(vid);
        Assert.Null(pid);
    }
}