namespace Atc.Wpf.Sample.SamplesWpfNetwork.Scanner;

[SuppressMessage("Security", "S1313:Do not hardcode IP addresses", Justification = "OK - Sample data only.")]
public class NetworkScannerSampleViewModel : ViewModelBase
{
    public NetworkScannerViewModel NetworkScanner { get; } = new()
    {
        StartIpAddress = "192.168.1.1",
        EndIpAddress = "192.168.1.254",
        PortsNumbers = [22, 80, 443],
    };
}