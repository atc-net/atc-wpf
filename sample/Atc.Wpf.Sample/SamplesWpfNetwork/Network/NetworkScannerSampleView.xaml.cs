namespace Atc.Wpf.Sample.SamplesWpfNetwork.Network;

public partial class NetworkScannerSampleView
{
    public NetworkScannerSampleView()
    {
        InitializeComponent();

        DataContext = new NetworkScannerSampleViewModel();
    }
}