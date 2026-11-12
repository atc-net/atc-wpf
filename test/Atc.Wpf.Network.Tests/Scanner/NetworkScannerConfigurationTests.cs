namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkScannerConfigurationTests
{
    [StaFact]
    public async Task Scan_UsesTheScannerFactory_AndScansTheRange()
    {
        FakeIpScanner? scanner = null;
        using var sut = CreateSut(config => scanner = new FakeIpScanner(config));

        await sut.ScanCommand.ExecuteAsync(null);

        Assert.NotNull(scanner);
        Assert.Equal(IPAddress.Parse("10.0.0.1"), scanner.ScannedStart);
        Assert.Equal(IPAddress.Parse("10.0.0.3"), scanner.ScannedEnd);
    }

    [StaFact]
    public async Task Scan_DisposesTheScanner()
    {
        FakeIpScanner? scanner = null;
        using var sut = CreateSut(config => scanner = new FakeIpScanner(config));

        await sut.ScanCommand.ExecuteAsync(null);

        Assert.True(scanner!.IsDisposed);
    }

    [StaFact]
    public async Task Scan_PassesThePortNumbersToTheScanner()
    {
        FakeIpScanner? scanner = null;
        using var sut = CreateSut(config => scanner = new FakeIpScanner(config));
        sut.PortsNumbers = [80, 554];

        await sut.ScanCommand.ExecuteAsync(null);

        Assert.Equal([80, 554], scanner!.Configuration.PortNumbers);
    }

    [StaFact]
    public void CreateScannerConfig_ByDefault_KeepsTheBehaviourOfEarlierVersions()
    {
        using var sut = new NetworkScannerViewModel();

        var config = sut.CreateScannerConfig();

        Assert.True(config.IcmpPing);
        Assert.True(config.ResolveHostName);
        Assert.True(config.ResolveMacAddress);
        Assert.True(config.ResolveVendorFromMacAddress);
        Assert.False(config.ResolveNetBiosName);
        Assert.False(config.DiscoverServices);
        Assert.False(config.DiscoverOnvif);
        Assert.Equal(IPServicePortExaminationLevel.WellKnown, config.TreatOpenPortsAsWebServices);
    }

    [StaFact]
    public void CreateScannerConfig_UsesTheOptions()
    {
        using var sut = new NetworkScannerViewModel();
        sut.Options.ResolveHostName = false;
        sut.Options.ResolveNetBiosName = true;
        sut.Options.DiscoverServices = true;
        sut.Options.DiscoverOnvif = true;
        sut.Options.ResolveVendorFromMacAddress = false;
        sut.Options.TimeoutPingInMs = 300;
        sut.Options.TimeoutDiscoveryInMs = 1500;
        sut.Options.TimeoutNetBiosInMs = 400;

        var config = sut.CreateScannerConfig();

        Assert.False(config.ResolveHostName);
        Assert.True(config.ResolveNetBiosName);
        Assert.True(config.DiscoverServices);
        Assert.True(config.DiscoverOnvif);
        Assert.False(config.ResolveVendorFromMacAddress);
        Assert.Equal(TimeSpan.FromMilliseconds(300), config.TimeoutPing);
        Assert.Equal(TimeSpan.FromMilliseconds(1500), config.TimeoutDiscovery);
        Assert.Equal(TimeSpan.FromMilliseconds(400), config.TimeoutNetBios);
    }

    [StaFact]
    public void CreateScannerConfig_RunsConfigureScannerLast()
    {
        using var sut = new NetworkScannerViewModel
        {
            ConfigureScanner = config => config.DiscoverOnvif = true,
        };
        sut.Options.DiscoverOnvif = false;

        var config = sut.CreateScannerConfig();

        Assert.True(config.DiscoverOnvif);
    }

    [StaFact]
    public void ScannerFactory_ByDefault_CreatesAnIpScannerWithTheConfig()
    {
        using var sut = new NetworkScannerViewModel();
        var config = new IPScannerConfig();

        var scanner = sut.ScannerFactory(config);

        Assert.IsType<IPScanner>(scanner);
        Assert.Same(config, scanner.Configuration);
        ((IDisposable)scanner).Dispose();
    }

    private static NetworkScannerViewModel CreateSut(
        Func<IPScannerConfig, IIPScanner> scannerFactory)
        => new()
        {
            StartIpAddress = "10.0.0.1",
            EndIpAddress = "10.0.0.3",
            ScannerFactory = scannerFactory,
        };
}