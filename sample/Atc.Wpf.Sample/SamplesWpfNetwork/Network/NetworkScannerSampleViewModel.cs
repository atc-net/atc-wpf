namespace Atc.Wpf.Sample.SamplesWpfNetwork.Network;

public class NetworkScannerSampleViewModel : ViewModelBase
{
    public NetworkScannerViewModel NetworkScanner { get; } = new()
    {
        StartIpAddress = "192.168.1.1",
        EndIpAddress = "192.168.1.254",
        PortsNumbers = [22, 80, 443],
    };
}