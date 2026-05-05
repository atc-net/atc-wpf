namespace Atc.Wpf.Hardware.Tests.Services.Internal;

public sealed class UsbDeviceClassFilterResolverTests
{
    [Fact]
    public void ToAqs_None_ReturnsBroadUsbInterface()
    {
        var aqs = UsbDeviceClassFilterResolver.ToAqs(UsbDeviceClassFilter.None);

        Assert.Equal(
            $"System.Devices.InterfaceClassGuid:=\"{UsbDeviceClassFilterResolver.UsbDeviceInterfaceGuid}\"",
            aqs);
    }

    [Theory]
    [InlineData(UsbDeviceClassFilter.Hid, UsbDeviceClassFilterResolver.HidInterfaceGuid)]
    [InlineData(UsbDeviceClassFilter.Imaging, UsbDeviceClassFilterResolver.ImagingInterfaceGuid)]
    [InlineData(UsbDeviceClassFilter.Audio, UsbDeviceClassFilterResolver.AudioInterfaceGuid)]
    [InlineData(UsbDeviceClassFilter.Printer, UsbDeviceClassFilterResolver.PrinterInterfaceGuid)]
    [InlineData(UsbDeviceClassFilter.MassStorage, UsbDeviceClassFilterResolver.MassStorageInterfaceGuid)]
    [InlineData(UsbDeviceClassFilter.Communication, UsbDeviceClassFilterResolver.ComPortInterfaceGuid)]
    public void ToAqs_SingleFlag_EmitsSingleClause(
        UsbDeviceClassFilter filter,
        string expectedGuid)
    {
        var aqs = UsbDeviceClassFilterResolver.ToAqs(filter);

        Assert.Equal(
            $"System.Devices.InterfaceClassGuid:=\"{expectedGuid}\"",
            aqs);
    }

    [Fact]
    public void ToAqs_MultipleFlags_EmitsOrJoinedClauses()
    {
        var aqs = UsbDeviceClassFilterResolver.ToAqs(
            UsbDeviceClassFilter.Hid | UsbDeviceClassFilter.Audio);

        Assert.Contains(UsbDeviceClassFilterResolver.HidInterfaceGuid, aqs, StringComparison.Ordinal);
        Assert.Contains(UsbDeviceClassFilterResolver.AudioInterfaceGuid, aqs, StringComparison.Ordinal);
        Assert.Contains(" OR ", aqs, StringComparison.Ordinal);
    }

    [Fact]
    public void ToAqs_AllFlags_EmitsSixOrJoinedClauses()
    {
        var all =
            UsbDeviceClassFilter.Hid |
            UsbDeviceClassFilter.Imaging |
            UsbDeviceClassFilter.Audio |
            UsbDeviceClassFilter.Printer |
            UsbDeviceClassFilter.MassStorage |
            UsbDeviceClassFilter.Communication;

        var aqs = UsbDeviceClassFilterResolver.ToAqs(all);

        var orCount = aqs.Split(" OR ", StringSplitOptions.None).Length - 1;
        Assert.Equal(5, orCount);
    }
}