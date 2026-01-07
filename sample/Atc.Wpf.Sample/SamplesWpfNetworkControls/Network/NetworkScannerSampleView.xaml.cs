namespace Atc.Wpf.Sample.SamplesWpfNetworkControls.Network;

public partial class NetworkScannerSampleView
{
    public NetworkScannerSampleView()
    {
        InitializeComponent();

        DataContext = new NetworkScannerSampleViewModel();
    }
}