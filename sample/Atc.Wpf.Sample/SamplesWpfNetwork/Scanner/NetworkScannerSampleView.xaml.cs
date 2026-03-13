namespace Atc.Wpf.Sample.SamplesWpfNetwork.Scanner;

public partial class NetworkScannerSampleView
{
    public NetworkScannerSampleView()
    {
        InitializeComponent();

        DataContext = new NetworkScannerSampleViewModel();
    }
}