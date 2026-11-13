namespace Atc.Wpf.Benchmarks;

/// <summary>
/// Measures the scanner's per-report and per-scan work. <c>Update_same_result</c> is what most progress reports cost
/// (the scanner reports a host many times); <c>New_host</c> is a host's first report; <c>Offline_hosts</c> is the
/// once-per-scan pass over remembered hosts, here with 1,000 remembered hosts that are all offline (its worst case).
/// </summary>
[MemoryDiagnoser]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "BenchmarkDotNet owns the instance; the scanner is disposed in GlobalCleanup.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "S3011:Reflection should not be used to increase accessibility", Justification = "Benchmarks a private once-per-scan method.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "S1313:Do not hardcode IP addresses", Justification = "Fixed test addresses.")]
public class NetworkScannerBenchmarks
{
    private const int KnownHostCount = 1_000;

    private static readonly MethodInfo UpdateOfflineHostsMethod = typeof(NetworkScannerViewModel)
        .GetMethod("UpdateOfflineHosts", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly FieldInfo KnownHostsField = typeof(NetworkScannerViewModel)
        .GetField("knownHosts", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private IPScanResult result = null!;
    private NetworkHostViewModel host = null!;
    private NetworkScannerViewModel scanner = null!;
    private object[] ranges = null!;

    [GlobalSetup]
    public void Setup()
    {
        var address = IPAddress.Parse("10.0.0.5");
        result = new IPScanResult(address)
        {
            PingStatus = new PingStatusResult(address, IPStatus.Success, pingInMs: 3),
            Hostname = "camera-05.lan",
            MacAddress = "00-11-22-33-44-05",
            MacVendor = "Vendor",
        };
        foreach (var port in new ushort[] { 22, 80, 443, 8080 })
        {
            result.Ports.Add(new IPScanPortResult { Port = port, TransportProtocol = TransportProtocolType.Tcp, CanConnect = true });
        }

        host = new NetworkHostViewModel(result);

        scanner = new NetworkScannerViewModel();
        var knownHosts = (Dictionary<IPAddress, NetworkKnownHost>)KnownHostsField.GetValue(scanner)!;
        for (var i = 0; i < KnownHostCount; i++)
        {
            var ip = new IPAddress([10, 0, (byte)(i / 256), (byte)(i % 256)]);
            var known = new NetworkKnownHost(ip, $"00-11-22-33-{i / 256:X2}-{i % 256:X2}", Name: null, DateTime.Now);
            knownHosts[ip] = known;
            scanner.OfflineHosts.Add(known);
        }

        IReadOnlyList<(IPAddress Start, IPAddress End)> scanned = [(IPAddress.Parse("10.0.0.0"), IPAddress.Parse("10.0.255.255"))];
        ranges = [scanned];
    }

    [GlobalCleanup]
    public void Cleanup()
        => scanner.Dispose();

    [Benchmark(Baseline = true)]
    [STAThread]
    public NetworkHostViewModel Update_same_result()
    {
        host.Update(result);
        return host;
    }

    [Benchmark]
    [STAThread]
    public NetworkHostViewModel New_host()
        => new(result);

    [Benchmark]
    [STAThread]
    public int Offline_hosts()
    {
        UpdateOfflineHostsMethod.Invoke(scanner, ranges);
        return scanner.OfflineHosts.Count;
    }
}