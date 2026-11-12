namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkHostViewModelDiscoveryTests
{
    private static readonly IPAddress Address = IPAddress.Parse("192.168.1.220");

    [Fact]
    public void DeviceName_PrefersTheDnsHostName()
    {
        var result = new IPScanResult(Address) { Hostname = "nas.lan" };
        result.MdnsServices.Add(Mdns("Printer._ipp._tcp.local", "_ipp._tcp.local", "printer.local"));

        var sut = new NetworkHostViewModel(result);

        Assert.Equal("nas.lan", sut.DeviceName);
    }

    [Fact]
    public void DeviceName_FallsBackToTheMdnsHostName()
    {
        var result = new IPScanResult(Address) { NetBiosStatus = NetBios("NAS1", group: null) };
        result.MdnsServices.Add(Mdns("Printer._ipp._tcp.local", "_ipp._tcp.local", "printer.local"));

        var sut = new NetworkHostViewModel(result);

        Assert.Equal("printer.local", sut.DeviceName);
    }

    [Fact]
    public void DeviceName_FallsBackToTheNetBiosName()
    {
        var result = new IPScanResult(Address) { NetBiosStatus = NetBios("NAS1", group: "WORKGROUP") };
        result.OnvifDevices.Add(Onvif("C210", "C210"));

        var sut = new NetworkHostViewModel(result);

        Assert.Equal("NAS1", sut.DeviceName);
        Assert.Equal("WORKGROUP", sut.Workgroup);
    }

    [Fact]
    public void DeviceName_FallsBackToTheOnvifName()
    {
        var result = new IPScanResult(Address);
        result.OnvifDevices.Add(Onvif("C210", "C210"));

        var sut = new NetworkHostViewModel(result);

        Assert.Equal("C210", sut.DeviceName);
    }

    [Fact]
    public void DeviceName_WithoutAnyName_IsNull()
    {
        var sut = new NetworkHostViewModel(new IPScanResult(Address));

        Assert.Null(sut.DeviceName);
    }

    [Fact]
    public void Model_PrefersTheOnvifHardware()
    {
        var result = new IPScanResult(Address);
        result.OnvifDevices.Add(Onvif("Camera", "DS-2CD2183G0-IU"));
        result.MdnsServices.Add(Mdns("HIKVISION DS-2CD2183G0-IU - F77653729._http._tcp.local", "_http._tcp.local", "cam.local"));

        var sut = new NetworkHostViewModel(result);

        Assert.Equal("DS-2CD2183G0-IU", sut.Model);
    }

    [Fact]
    public void Model_FallsBackToTheMdnsModelProperty()
    {
        var result = new IPScanResult(Address);
        result.MdnsServices.Add(Mdns(
            "Office._ipp._tcp.local",
            "_ipp._tcp.local",
            "printer.local",
            new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase) { ["ty"] = "HP LaserJet 400" }));

        var sut = new NetworkHostViewModel(result);

        Assert.Equal("HP LaserJet 400", sut.Model);
    }

    [Fact]
    public void Model_FallsBackToTheMdnsInstanceName()
    {
        var result = new IPScanResult(Address);
        result.MdnsServices.Add(Mdns("HIKVISION DS-2CD2183G0-IU - F77653729._psia._tcp.local", "_psia._tcp.local", "cam.local"));

        var sut = new NetworkHostViewModel(result);

        Assert.Equal("HIKVISION DS-2CD2183G0-IU - F77653729", sut.Model);
    }

    [Fact]
    public void Services_ListsTheMdnsServiceTypesAndUpnp()
    {
        var result = new IPScanResult(Address);
        result.MdnsServices.Add(Mdns("A._psia._tcp.local", "_psia._tcp.local", "cam.local"));
        result.MdnsServices.Add(Mdns("A._http._tcp.local", "_http._tcp.local", "cam.local"));
        result.MdnsServices.Add(Mdns("B._http._tcp.local", "_http._tcp.local", "cam.local"));
        result.SsdpDevices.Add(new SsdpDevice(Address, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["USN"] = "uuid:1" }));

        var sut = new NetworkHostViewModel(result);

        Assert.Equal(["_http._tcp", "_psia._tcp", "UPnP"], sut.Services);
        Assert.Equal("_http._tcp, _psia._tcp, UPnP", sut.ServicesAsCommaList);
    }

    [Fact]
    public void Onvif_TellsCamerasFromOtherOnvifDevices()
    {
        var camera = new IPScanResult(Address);
        camera.OnvifDevices.Add(Onvif("C210", "C210"));
        var recorder = new IPScanResult(Address);
        recorder.OnvifDevices.Add(Onvif("NVR", "NVR", isCamera: false));

        var cameraHost = new NetworkHostViewModel(camera);
        var recorderHost = new NetworkHostViewModel(recorder);

        Assert.True(cameraHost.IsOnvifDevice);
        Assert.True(cameraHost.IsOnvifCamera);
        Assert.Equal(new Uri("http://192.168.1.220/onvif/device_service"), cameraHost.OnvifDeviceServiceUrl);
        Assert.True(recorderHost.IsOnvifDevice);
        Assert.False(recorderHost.IsOnvifCamera);
    }

    [Fact]
    public void ToAllInfoText_IncludesTheDiscoveryFields()
    {
        var result = new IPScanResult(Address) { NetBiosStatus = NetBios("CAM1", group: "WORKGROUP") };
        result.OnvifDevices.Add(Onvif("C210", "C210"));
        result.MdnsServices.Add(Mdns("A._http._tcp.local", "_http._tcp.local", "cam.local"));
        var sut = new NetworkHostViewModel(result);

        var text = sut.ToAllInfoText();

        Assert.Contains("192.168.1.220", text, StringComparison.Ordinal);
        Assert.Contains("cam.local", text, StringComparison.Ordinal);
        Assert.Contains("WORKGROUP", text, StringComparison.Ordinal);
        Assert.Contains("C210", text, StringComparison.Ordinal);
        Assert.Contains("_http._tcp", text, StringComparison.Ordinal);
        Assert.Contains("http://192.168.1.220/onvif/device_service", text, StringComparison.Ordinal);
    }

    private static MdnsServiceInstance Mdns(
        string fullName,
        string serviceType,
        string hostName,
        IReadOnlyDictionary<string, string?>? properties = null)
        => new(
            fullName,
            serviceType,
            hostName,
            port: 80,
            [Address],
            properties ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase));

    private static NetBiosNodeStatus NetBios(
        string computerName,
        string? group)
    {
        var names = new List<NetBiosName> { new(computerName, 0x00, IsGroup: false) };
        if (group is not null)
        {
            names.Add(new NetBiosName(group, 0x00, IsGroup: true));
        }

        return new NetBiosNodeStatus(Address, names, macAddress: null);
    }

    private static OnvifDevice Onvif(
        string name,
        string hardware,
        bool isCamera = true)
        => new(
            Address,
            "urn:uuid:1",
            [isCamera ? "dn:NetworkVideoTransmitter" : "tds:Device"],
            [$"onvif://www.onvif.org/name/{name}", $"onvif://www.onvif.org/hardware/{hardware}"],
            [new Uri("http://192.168.1.220/onvif/device_service")]);
}