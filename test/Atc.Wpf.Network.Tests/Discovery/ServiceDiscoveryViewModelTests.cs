namespace Atc.Wpf.Network.Tests.Discovery;

public sealed class ServiceDiscoveryViewModelTests
{
    private static readonly IPAddress Camera = IPAddress.Parse("192.168.1.220");
    private static readonly IPAddress Printer = IPAddress.Parse("192.168.1.50");

    [StaFact]
    public async Task Discover_GroupsTheAnswersByAddress()
    {
        var discoverer = new FakeServiceDiscoverer
        {
            MdnsAnswers =
            [
                Mdns("HIKVISION DS-2CD2183G0-IU - F77653729._http._tcp.local", "_http._tcp.local", "cam.local", Camera),
                Mdns("HIKVISION DS-2CD2183G0-IU - F77653729._psia._tcp.local", "_psia._tcp.local", "cam.local", Camera),
                Mdns("Office._ipp._tcp.local", "_ipp._tcp.local", "printer.local", Printer),
            ],
            OnvifAnswers = [Onvif(Camera)],
        };
        var sut = new ServiceDiscoveryViewModel(discoverer);

        await sut.DiscoverCommand.ExecuteAsync(null);

        Assert.Equal(2, sut.Devices.Count);
        var camera = sut.Devices.Single(x => x.IpAddress.Equals(Camera));
        Assert.Equal("cam.local", camera.DeviceName);
        Assert.Equal("DS-2CD2183G0-IU", camera.Model);
        Assert.Equal(["_http._tcp", "_psia._tcp"], camera.Services, StringComparer.Ordinal);
        Assert.True(camera.IsOnvifCamera);
        Assert.Equal("printer.local", sut.Devices.Single(x => x.IpAddress.Equals(Printer)).DeviceName);
    }

    [StaFact]
    public async Task Discover_SortsTheDevicesByAddress()
    {
        var discoverer = new FakeServiceDiscoverer
        {
            SsdpAnswers = [Ssdp(Camera), Ssdp(Printer), Ssdp(IPAddress.Parse("192.168.1.9"))],
        };
        var sut = new ServiceDiscoveryViewModel(discoverer);

        await sut.DiscoverCommand.ExecuteAsync(null);

        Assert.Equal(
            ["192.168.1.9", "192.168.1.50", "192.168.1.220"],
            CollectionViewSource.GetDefaultView(sut.Devices).Cast<NetworkHostViewModel>().Select(x => x.IpAddress.ToString()),
            StringComparer.Ordinal);
    }

    [StaFact]
    public async Task Discover_RunsOnlyTheChosenProtocols_ForTheChosenDuration()
    {
        var discoverer = new FakeServiceDiscoverer();
        var sut = new ServiceDiscoveryViewModel(discoverer)
        {
            UseMdns = false,
            UseSsdp = true,
            UseOnvif = false,
            DurationInMs = 1500,
        };

        await sut.DiscoverCommand.ExecuteAsync(null);

        Assert.Equal(["ssdp"], discoverer.Calls);
        Assert.Equal(TimeSpan.FromMilliseconds(1500), discoverer.Duration);
    }

    [StaFact]
    public async Task Discover_Again_StartsWithAnEmptyList()
    {
        var discoverer = new FakeServiceDiscoverer { SsdpAnswers = [Ssdp(Camera)] };
        var sut = new ServiceDiscoveryViewModel(discoverer);
        await sut.DiscoverCommand.ExecuteAsync(null);
        discoverer.SsdpAnswers = [Ssdp(Printer)];

        await sut.DiscoverCommand.ExecuteAsync(null);

        Assert.Equal(Printer, Assert.Single(sut.Devices).IpAddress);
    }

    [StaFact]
    public async Task Discover_RaisesDeviceFoundOncePerAddress_AndDiscoveryCompleted()
    {
        var discoverer = new FakeServiceDiscoverer
        {
            MdnsAnswers = [Mdns("A._http._tcp.local", "_http._tcp.local", "cam.local", Camera)],
            OnvifAnswers = [Onvif(Camera)],
        };
        var sut = new ServiceDiscoveryViewModel(discoverer);
        var found = new List<NetworkHostViewModel>();
        var completed = 0;
        sut.DeviceFound += (_, e) => found.Add(e.Host);
        sut.DiscoveryCompleted += (_, _) => completed++;

        await sut.DiscoverCommand.ExecuteAsync(null);

        Assert.Same(sut.Devices[0], Assert.Single(found));
        Assert.Equal(1, completed);
    }

    [StaFact]
    public async Task Discover_WhenAProtocolFails_ShowsTheErrorAndKeepsTheOthers()
    {
        var discoverer = new FakeServiceDiscoverer
        {
            SsdpAnswers = [Ssdp(Camera)],
            OnvifException = new System.Net.Sockets.SocketException(10049),
        };
        var sut = new ServiceDiscoveryViewModel(discoverer);

        await sut.DiscoverCommand.ExecuteAsync(null);

        Assert.Single(sut.Devices);
        Assert.True(sut.HasError);
    }

    [Fact]
    public void Discover_WithoutAnyProtocol_IsDisabled()
    {
        var sut = new ServiceDiscoveryViewModel(new FakeServiceDiscoverer())
        {
            UseMdns = false,
            UseSsdp = false,
            UseOnvif = false,
        };

        Assert.False(sut.DiscoverCommand.CanExecute(null));
    }

    private static MdnsServiceInstance Mdns(
        string fullName,
        string serviceType,
        string hostName,
        IPAddress address)
        => new(fullName, serviceType, hostName, 80, [address], new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase));

    private static SsdpDevice Ssdp(IPAddress address)
        => new(address, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["USN"] = $"uuid:{address}" });

    private static OnvifDevice Onvif(IPAddress address)
        => new(
            address,
            "urn:uuid:1",
            ["dn:NetworkVideoTransmitter"],
            ["onvif://www.onvif.org/name/Camera", "onvif://www.onvif.org/hardware/DS-2CD2183G0-IU"],
            [new Uri($"http://{address}/onvif/device_service")]);

    private sealed class FakeServiceDiscoverer : IServiceDiscoverer
    {
        public IList<MdnsServiceInstance> MdnsAnswers { get; set; } = [];

        public IList<SsdpDevice> SsdpAnswers { get; set; } = [];

        public IList<OnvifDevice> OnvifAnswers { get; set; } = [];

        public Exception? OnvifException { get; set; }

        public IList<string> Calls { get; } = new List<string>();

        public TimeSpan? Duration { get; private set; }

        public IAsyncEnumerable<MdnsServiceInstance> BrowseMdns(
            TimeSpan duration,
            CancellationToken cancellationToken)
        {
            Calls.Add("mdns");
            Duration = duration;
            return MdnsAnswers.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<SsdpDevice> SearchSsdp(
            TimeSpan duration,
            CancellationToken cancellationToken)
        {
            Calls.Add("ssdp");
            Duration = duration;
            return SsdpAnswers.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<OnvifDevice> ProbeOnvif(
            TimeSpan duration,
            CancellationToken cancellationToken)
        {
            Calls.Add("onvif");
            Duration = duration;
            return OnvifException is null
                ? OnvifAnswers.ToAsyncEnumerable()
                : Throw(OnvifException);
        }

        private static async IAsyncEnumerable<OnvifDevice> Throw(
            Exception exception)
        {
            await Task.CompletedTask;
            throw exception;
#pragma warning disable CS0162 // Unreachable code: the iterator needs a yield.
            yield break;
#pragma warning restore CS0162
        }
    }
}